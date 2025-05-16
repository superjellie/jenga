#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Jenga {
	public class TypeSelectorField : VisualElement {
	    
	    //
		public const string ussClassName = "jenga-type-selector";

		public delegate void OnSelectDelegate(System.Type type);

		//
		public System.Type typeFamily;
		public System.Type currentType;
		public OnSelectDelegate onSelect = (t) => { };
		public SerializedProperty property;

		public TypeSelectorField() {

			EnableInClassList(ussClassName, true);

			RegisterCallback<AttachToPanelEvent>(
				OnAttachToPanel, TrickleDown.TrickleDown
			);

		}

		void OnAttachToPanel(AttachToPanelEvent evt) {
			var registry = AddTypeMenuAttribute.registries[typeFamily];

			var field = new PopupField<AddTypeMenuAttribute.Registration>() {
				choices = registry,
				formatListItemCallback = (reg) => reg.path,
				formatSelectedValueCallback 
					= (reg) => reg.path?.Substring(reg.path.LastIndexOf('/') + 1),
				index = registry.FindIndex((reg) => reg.type == currentType)
			};

			Add(field);

			field.RegisterCallback<ChangeEvent<AddTypeMenuAttribute.Registration>>(
				(evt) => {
					if (property != null) {
	                	SerializedPropertyUtility
	                    	.SetManagedReference(property, evt.newValue.type);
	               		property.serializedObject.ApplyModifiedProperties();
					}

					onSelect(evt.newValue.type);
				}
			);

			this.AddManipulator(new ContextualMenuManipulator((evt) => {
	            evt.menu.AppendAction(
	            	"Edit Script", 
	            	(x) => OpenScriptAsset
	            		(registry.Find((reg) => reg.type == currentType)), 
	            	DropdownMenuAction.AlwaysEnabled
	            );

	            evt.menu.AppendAction(
	            	"Copy", (x) => CopyToClipboard(), 
	            	DropdownMenuAction.AlwaysEnabled
	            );

	            evt.menu.AppendAction(
	            	"Paste", 
	            	(x) => {
	            		var value = ParseFromClipboard();

						if (property != null) {
			            	property.managedReferenceValue = value;
			           		property.serializedObject.ApplyModifiedProperties();
			           		property.serializedObject.Update();
							onSelect(value.GetType());
						}
	            	}, 
	            	(x) => ParseFromClipboard() != null
	            		? DropdownMenuAction.Status.Normal
	            		: DropdownMenuAction.Status.Disabled
	            );

				// Debug.Log($"{registry.Count}");
	            foreach (var reg in registry) {
	            	var itemField = reg.type.GetField("item");
	            	var valueField = itemField?.FieldType.GetField("value");
	            	// Debug.Log($"{reg.type}: {itemField}");
	            	if (itemField == null || valueField == null) continue;

		            evt.menu.AppendAction(
		            	$"Wrap With/{reg.path}", 
		            	(x) => {
		            		var newValue = System.Activator
		            			.CreateInstance(reg.type);
		            		var newValueRef = System.Activator
		            			.CreateInstance(itemField?.FieldType);
		            			
		            		var oldValue = property.managedReferenceValue;

		            		valueField.SetValue(newValueRef, oldValue);
		            		itemField.SetValue(newValue, newValueRef);

			            	property.managedReferenceValue = newValue;
			           		property.serializedObject.ApplyModifiedProperties();
			           		property.serializedObject.Update();
							onSelect(newValue.GetType());
		            	}
		            );
	            }

	            if (currentType != null) {
	            	var myItemField = currentType.GetField("item");
	            	var myValueField = myItemField?.FieldType.GetField("value");

	            	if (myItemField != null && myValueField != null) 
			            evt.menu.AppendAction(
			            	$"Replace With Child", 
			            	(x) => {
			            		var value = property.managedReferenceValue;
			            		var valueRef = myItemField.GetValue(value);
			            		var child = myValueField.GetValue(valueRef);

				            	property.managedReferenceValue = child;
				           		property.serializedObject.ApplyModifiedProperties();
				           		property.serializedObject.Update();
								onSelect(child?.GetType());
			            	}
			            );
	            }


	        }));
		}

		void OpenScriptAsset(AddTypeMenuAttribute.Registration reg) { 
			if (reg.type == null) 
				return;

			var asset = AssetDatabase.LoadMainAssetAtPath(reg.pathToSource);
			
			if (asset == null) {
				Debug.LogWarning($"Could not find asset as {reg.pathToSource}");
				return;
			}

			AssetDatabase.OpenAsset(asset, reg.sourceLineNumber); 
		}

		void CopyToClipboard() { 
			if (property == null 
				|| property.propertyType != SerializedPropertyType.ManagedReference)
				return;

			EditorGUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(
				property.managedReferenceValue,
				Formatting.Indented,
				new JsonSerializerSettings() { 
					Error = (s, a) => { 
						// Debug.Log(a.ErrorContext.Error.Message); 
						a.ErrorContext.Handled = true; 
					}, 
					PreserveReferencesHandling 
						= PreserveReferencesHandling.Objects,
					TypeNameHandling = TypeNameHandling.Objects,
					ContractResolver = new ClipboardResolver() 
				}
			);
		}

		object ParseFromClipboard() {
			return JsonConvert.DeserializeObject(
				EditorGUIUtility.systemCopyBuffer,
				typeFamily,
				new JsonSerializerSettings() { 
					Error = (s, a) => { 
						// Debug.Log(a.ErrorContext.Error.Message); 
						a.ErrorContext.Handled = true; 
					}, 
					PreserveReferencesHandling 
						= PreserveReferencesHandling.Objects,
					TypeNameHandling = TypeNameHandling.Objects,
					ContractResolver = new ClipboardResolver()
				}
			);
		}

		class UnityObjectConverter : JsonConverter {
	    	public override void WriteJson(
	    		JsonWriter writer, object value, JsonSerializer serializer
	    	) {
		        writer.WriteValue((value as Object).GetInstanceID());
	     	}

	    	public override object ReadJson(
	    		JsonReader reader, System.Type objectType, object existingValue, 
	    		JsonSerializer serializer
	    	){
	    		// Debug.Log($"{reader.Value}, {reader.ValueType}");
	    		var instanceID = (System.Int64)reader.Value;
	    		return EditorUtility.InstanceIDToObject((int)instanceID);
	    	}

		    public override bool CanConvert(System.Type objectType) {
		        return objectType.IsSubclassOf(typeof(Object));
		    }

		}

		class ClipboardResolver : DefaultContractResolver {
		    protected override JsonContract CreateContract(System.Type objectType) {
		        JsonContract contract = base.CreateContract(objectType);
		        if (objectType.IsSubclassOf(typeof(Object)))
		            contract.Converter = new UnityObjectConverter();
		        
		        return contract;
		    }
		}

	}
}
#endif