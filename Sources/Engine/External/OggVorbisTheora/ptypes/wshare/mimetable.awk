#
#
#  C++ Portable Types Library (PTypes)
#  Version 2.1.1  Released 27-Jun-2007
#
#  Copyright (C) 2001-2007 Hovik Melikyan
#
#  http://www.melikyan.com/ptypes/
#
#

#
# Convert Apache's mime.types file to C++ declaration
#

BEGIN {
    printf "//\n// Generated from Apache's mime.types file by mimetypes.awk\n//\n\n\
const char* mimetypes[] = {\n";
}

NF > 1 && substr($0, 0, 1) != "#" {
    printf "    ";
    for (i = 2; i <= NF; i++)
	printf "\".%s\", ", $i;
    printf "\"%s\",\n", $1;
}


END {
    printf "    0,\n};\n";
}

