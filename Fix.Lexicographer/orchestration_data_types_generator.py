#!/usr/bin/python3

from sanitise import *
from pedigree import *

def generate_orchestration_data_types(namespace, prefix, orchestration):
    sane_prefix = sanitise_for_include_guard(prefix)
    filename = '{}DataTypes.cs'.format(prefix)
    with open(filename, 'w') as file:
        file.write('namespace Fix\n{\n')
        file.write('    public partial class Dictionary\n    {\n')
        file.write('        public partial class {}\n        {{\n'.format(namespace))
        file.write('            public class DataTypeCollection : VersionDataTypeCollection\n')
        file.write('            {\n')

        data_types = []
        for data_type in orchestration.data_types.values():
            name = data_type.name
            if name in ('char', 'int', 'float', 'double'):
                name = name[0].upper() + name[1:]
            data_types.append((name, sanitise(data_type.synopsis), data_type.pedigree))
        
        file.write('            public DataTypeCollection()\n')
        file.write('            {\n')
        file.write('                    DataTypes = new Dictionary.DataType[] {{{}}};\n'.format(', '.join([name for (name, _, _) in data_types])))
        file.write('            }\n')

        for (name, synopsis, pedigree) in data_types:
            file.write('                public readonly DataType {} = new DataType("{}", "{}", {});\n'.format(name, name, synopsis, format_pedigree(pedigree)))
        
        file.write('            }\n')

        file.write('\n')
        file.write('            public static DataTypeCollection DataTypes { get; } = new DataTypeCollection();\n')

        file.write('        }\n')
        file.write('    }\n')
        file.write('}\n')