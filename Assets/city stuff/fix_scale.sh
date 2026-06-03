#!/bin/bash
# fix_scale.sh
# Recursively finds all .prefab files and replaces:
#   "Scale": "40,40,40",  →  "Scale": "0.4,0.4,0.4",
# Files are overwritten in place.

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PATTERN='"Scale": "40,40,40",'
REPLACEMENT='"Scale": "0.4,0.4,0.4",'
COUNT=0

echo "========================================"
echo "  Prefab Scale Fixer"
echo "  Directory: $SCRIPT_DIR"
echo "========================================"
echo ""

while IFS= read -r -d '' file; do
    if grep -qF "$PATTERN" "$file"; then
        sed -i "s|\"Scale\": \"40,40,40\",|\"Scale\": \"0.4,0.4,0.4\",|g" "$file"
        echo "✓ Fixed: ${file#$SCRIPT_DIR/}"
        COUNT=$((COUNT + 1))
    fi
done < <(find "$SCRIPT_DIR" -type f -name "*.prefab" -print0)

echo ""
echo "========================================"
echo "  Done. $COUNT file(s) updated."
echo "========================================"
