#!/bin/sh
#
# Convert EOL character to Linux format
#
if [ $# -ne 1 ]; then
	echo "Usage: "
	echo "$0 <file-name>"
	exit 1
fi
sed -i '' -e "s///g" $1
