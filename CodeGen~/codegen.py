import os
dir_path = os.path.dirname(os.path.realpath(__file__))
source = os.path.join(dir_path, "./jenga/")
target = os.path.join(dir_path, "../")

for subdir, dirs, files in os.walk(source):
	for file in files:
		name, ext = os.path.splitext(file)
		if ext != ".py": continue
		# print(name, ext)
		path = os.path.join(subdir, file)
		# print(path)
		relpath = os.path.relpath(path, start = source)
		# print(relpath)
		newpath = os.path.join(target, relpath)
		newname, _ = os.path.splitext(newpath)
		newpath = f"{newname}.cs";
		newdir = os.path.dirname(newpath)
		# print(newpath)

		# print(f"if not exist \"{newdir}\" mkdir \"{newdir}\"")
		os.system(f"if not exist \"{newdir}\" mkdir \"{newdir}\"")
		print(f"CMD> py \"{path}\" > \"{newpath}\"")
		os.system(f"py \"{path}\" > \"{newpath}\"")


