import itertools

types = {
	"float": { 
		"name": "Float",
		"dim": 1, 
		"coords": ["0", "1"],
		"impl": [" 0f", " 1f"]
	},
	"Vector2": { 
		"name": "Vec",
		"dim": 2, 
		"coords": ["0", "1", "X", "Y"],
		"impl": [" 0f", " 1f", "v.x", "v.y"]
	},
	"Vector3": { 
		"name": "Vec",
		"dim": 3, 
		"coords": ["0", "1", "X", "Y", "Z"],
		"impl": [" 0f", " 1f", "v.x", "v.y", "v.z"]
	},
	"Vector4": { 
		"name": "Vec",
		"dim": 4, 
		"coords": ["0", "1", "X", "Y", "Z", "W"],
		"impl": [" 0f", " 1f", "v.x", "v.y", "v.z", "v.w"]
	},
	"Color": { 
		"name": "Color",
		"dim": 4, 
		"coords": ["0", "1", "R", "G", "B", "A"],
		"impl": [" 0f", " 1f", "v.r", "v.g", "v.b", "v.a"]
	},
	"int": { 
		"name": "Int",
		"dim": 1, 
		"coords": ["0", "1"],
		"impl": ["  0", "  1"]
	},
	"Vector2Int": { 
		"name": "Int",
		"dim": 2, 
		"coords": ["0", "1", "X", "Y"],
		"impl": ["  0", "  1", "v.x", "v.y"]
	},
	"Vector3Int": { 
		"name": "Int",
		"dim": 3, 
		"coords": ["0", "1", "X", "Y", "Z"],
		"impl": ["  0", "  1", "v.x", "v.y", "v.z"]
	},
	"Vector4Int": { 
		"name": "Int",
		"dim": 4, 
		"coords": ["0", "1", "X", "Y", "Z", "W"],
		"impl": ["  0", "  1", "v.x", "v.y", "v.z", "v.w"]
	}
}

for (fromTp, fromParams) in types.items():
	for (toTp, toParams) in types.items():
		dim = toParams["dim"]
		pairs = zip(fromParams["coords"], fromParams["impl"])
		combs = itertools.product(pairs, repeat = dim)
		for comb in combs:
			myNames, myValues = zip(*comb)
			print(f"		public static {toTp} {toParams['name']}{''.join(myNames)}"
				+ f"({fromTp} v) => {toParams['name']}{toParams['dim']}({', '.join(myValues)});") 