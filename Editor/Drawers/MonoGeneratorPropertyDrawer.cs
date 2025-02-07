#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// using UnityEditor.Callbacks;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Jenga {
	[CustomPropertyDrawer(typeof(MonoGeneratorReference<>), true)]
	public class MonoGeneratorReferencePropertyDrawer : PropertyDrawer {

	    public override VisualElement CreatePropertyGUI(SerializedProperty prop) {

	    	return new ManagedReferenceField() {
	    		property = prop.FindPropertyRelative("serializedValue"),
	    		typeFamily 
		        	= typeof(MonoGenerator<>)
			        	.MakeGenericType(
			        		SerializedPropertyUtility.GetFieldType(fieldInfo)
			        			.GenericTypeArguments[0]
			        	),
			    labelText = preferredLabel
	    	};
	    }

	}
}
#endif