"""Copy every mesh object from the House Interior Pack blend files into one file."""

from pathlib import Path

import bpy
from mathutils import Vector


INPUT_DIR = Path(
    r"C:\repos\RobotCleaner Assets\ultimate_house_interior_pack_-_june_2020"
    r"\Ultimate House Interior Pack - June 2020\Blends"
)
OUTPUT_PATH = (
    Path(
        r"C:\repos\RobotCleaner Assets\ultimate_house_interior_pack_-_june_2020"
    )
    / "Ultimate House Interior Pack - June 2020"
    / "all_house_meshes.blend"
)
LAYOUT_WIDTH = 50.0
LAYOUT_SPACING = 1.0


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)

    for datablocks in (bpy.data.meshes, bpy.data.objects, bpy.data.collections):
        for datablock in list(datablocks):
            if datablock.users == 0:
                datablocks.remove(datablock)


def copy_meshes_from_blend(source_path: Path) -> list:
    copied_objects = []

    with bpy.data.libraries.load(str(source_path), link=False) as (data_from, data_to):
        data_to.objects = data_from.objects

    for source_object in data_to.objects:
        if source_object is None or source_object.type != "MESH":
            continue

        source_object.data = source_object.data.copy()
        bpy.context.scene.collection.objects.link(source_object)
        copied_objects.append(source_object)

    return copied_objects


def arrange_meshes(mesh_objects):
    cursor_x = 0.0
    row_y = 0.0
    row_height = 0.0

    for mesh_object in mesh_objects:
        world_corners = [
            mesh_object.matrix_world @ Vector(corner)
            for corner in mesh_object.bound_box
        ]
        min_x = min(corner.x for corner in world_corners)
        max_x = max(corner.x for corner in world_corners)
        min_y = min(corner.y for corner in world_corners)
        max_y = max(corner.y for corner in world_corners)
        width = max_x - min_x
        height = max_y - min_y

        if cursor_x > 0 and cursor_x + width > LAYOUT_WIDTH:
            cursor_x = 0.0
            row_y -= row_height + LAYOUT_SPACING
            row_height = 0.0

        mesh_object.matrix_world.translation += Vector(
            (cursor_x - min_x, row_y - min_y, 0.0)
        )
        cursor_x += width + LAYOUT_SPACING
        row_height = max(row_height, height)


def main():
    if not INPUT_DIR.is_dir():
        raise FileNotFoundError(f"Input directory does not exist: {INPUT_DIR}")

    source_files = sorted(INPUT_DIR.rglob("*.blend"))
    if not source_files:
        raise FileNotFoundError(f"No blend files found in: {INPUT_DIR}")

    OUTPUT_PATH.parent.mkdir(parents=True, exist_ok=True)
    clear_scene()

    mesh_objects = []
    for source_path in source_files:
        copied_objects = copy_meshes_from_blend(source_path)
        mesh_objects.extend(copied_objects)
        print(f"Copied {len(copied_objects)} mesh object(s) from {source_path}")

    arrange_meshes(mesh_objects)

    bpy.ops.wm.save_as_mainfile(filepath=str(OUTPUT_PATH))
    print(f"Saved {len(mesh_objects)} mesh object(s) to {OUTPUT_PATH}")


if __name__ == "__main__":
    main()