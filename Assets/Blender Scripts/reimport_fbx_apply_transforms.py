"""Re-import every FBX in the Kitchen Extra folder and export transformed copies."""

from pathlib import Path

import bpy


INPUT_DIR = Path(r"C:\repos\robot-cleaner-project\Assets\Download tests\ok_Kitchen_1.0_Extra\Kitchen_Extra\FBX")
OUTPUT_DIR = Path(r"C:\repos\robot-cleaner-project\Assets\Download tests\ok_Kitchen_1.0_Extra\Kitchen_Extra\FBX Reimported")


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)

    for datablocks in (bpy.data.meshes, bpy.data.curves, bpy.data.materials, bpy.data.cameras, bpy.data.lights):
        for datablock in list(datablocks):
            if datablock.users == 0:
                datablocks.remove(datablock)


def import_and_export(source_path: Path):
    clear_scene()
    bpy.ops.import_scene.fbx(filepath=str(source_path))

    mesh_objects = [obj for obj in bpy.context.scene.objects if obj.type == "MESH"]
    if not mesh_objects:
        print(f"Skipping {source_path.name}: no mesh objects found")
        return

    bpy.ops.object.select_all(action="DESELECT")
    for obj in mesh_objects:
        obj.hide_set(False)
        obj.hide_viewport = False
        obj.hide_render = False
        obj.select_set(True)

    bpy.context.view_layer.objects.active = mesh_objects[0]
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)

    output_path = OUTPUT_DIR / source_path.name
    bpy.ops.export_scene.fbx(
        filepath=str(output_path),
        use_selection=True,
        object_types={"MESH"},
        bake_space_transform=True,
        add_leaf_bones=False,
    )
    print(f"Exported {output_path}")


def main():
    if not INPUT_DIR.is_dir():
        raise FileNotFoundError(f"Input directory does not exist: {INPUT_DIR}")

    OUTPUT_DIR.mkdir(parents=True, exist_ok=True)
    source_files = sorted(INPUT_DIR.glob("*.fbx"))
    if not source_files:
        raise FileNotFoundError(f"No FBX files found in: {INPUT_DIR}")

    for source_path in source_files:
        import_and_export(source_path)

    print(f"Finished processing {len(source_files)} FBX file(s)")


if __name__ == "__main__":
    main()