"""
FBX Triangulate Batch Processor
--------------------------------
Place this script in a directory. Run it via Blender:

    blender --background --python fbx_triangulate.py

The script will:
  1. Create a "bin/" folder next to itself.
  2. Recursively scan all subdirectories for .fbx files.
  3. Import each FBX into Blender, apply Triangulate Face modifier,
     then export it into "bin/" mirroring the original folder structure.
"""

import bpy
import os
import sys


# ── Helpers ──────────────────────────────────────────────────────────────────

def get_script_dir() -> str:
    """Return the directory that contains this script."""
    # sys.argv layout when called via:
    #   blender --background --python fbx_triangulate.py
    # is: ['blender', '--background', '--python', 'fbx_triangulate.py', ...]
    for i, arg in enumerate(sys.argv):
        if arg == "--python" and i + 1 < len(sys.argv):
            script_path = sys.argv[i + 1]
            return os.path.dirname(os.path.abspath(script_path))
    # Fallback: current working directory
    return os.path.abspath(os.getcwd())


def clear_scene():
    """Delete everything in the current Blender scene."""
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=True)
    # Also purge orphaned data blocks so memory stays clean
    bpy.ops.outliner.orphans_purge(
        do_local_ids=True, do_linked_ids=True, do_recursive=True
    )


def import_fbx(filepath: str):
    """Import an FBX file into the current scene."""
    bpy.ops.import_scene.fbx(filepath=filepath)


def triangulate_all_meshes():
    """
    Add a Triangulate modifier to every mesh object in the scene,
    apply it, then remove it — leaving only triangulated geometry.
    """
    for obj in bpy.context.scene.objects:
        if obj.type != "MESH":
            continue

        # Add modifier
        mod = obj.modifiers.new(name="Triangulate", type="TRIANGULATE")
        mod.quad_method = "BEAUTY"
        mod.ngon_method = "BEAUTY"

        # Make the object active so we can apply the modifier
        bpy.context.view_layer.objects.active = obj
        obj.select_set(True)
        bpy.ops.object.modifier_apply(modifier=mod.name)
        obj.select_set(False)


def export_fbx(filepath: str):
    """Export the entire scene as an FBX file."""
    os.makedirs(os.path.dirname(filepath), exist_ok=True)
    bpy.ops.export_scene.fbx(
        filepath=filepath,
        use_selection=False,          # export everything
        mesh_smooth_type="FACE",
        use_triangles=True,           # belt-and-suspenders: also flag on export
        add_leaf_bones=False,
    )


# ── Main ─────────────────────────────────────────────────────────────────────

def main():
    script_dir = get_script_dir()
    bin_dir    = os.path.join(script_dir, "bin")
    os.makedirs(bin_dir, exist_ok=True)

    print(f"\n{'='*60}")
    print(f"  FBX Triangulate Processor")
    print(f"  Source : {script_dir}")
    print(f"  Output : {bin_dir}")
    print(f"{'='*60}\n")

    # Collect every .fbx file under script_dir (skip the bin folder itself)
    fbx_files = []
    for root, dirs, files in os.walk(script_dir):
        # Skip the bin output directory
        dirs[:] = [d for d in dirs if os.path.join(root, d) != bin_dir]

        for filename in files:
            if filename.lower().endswith(".fbx"):
                fbx_files.append(os.path.join(root, filename))

    if not fbx_files:
        print("No .fbx files found. Nothing to do.")
        return

    print(f"Found {len(fbx_files)} FBX file(s).\n")

    for i, src_path in enumerate(fbx_files, start=1):
        # Build mirrored output path inside bin/
        rel_path  = os.path.relpath(src_path, script_dir)   # e.g. subdir/model.fbx
        dst_path  = os.path.join(bin_dir, rel_path)

        print(f"[{i}/{len(fbx_files)}] Processing: {rel_path}")
        print(f"            → {dst_path}")

        try:
            clear_scene()
            import_fbx(src_path)
            triangulate_all_meshes()
            export_fbx(dst_path)
            print(f"            ✓ Done\n")
        except Exception as exc:
            print(f"            ✗ ERROR: {exc}\n")

    print("="*60)
    print(f"  Finished. Results are in: {bin_dir}")
    print("="*60)


if __name__ == "__main__":
    main()