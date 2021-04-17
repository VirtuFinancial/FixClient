#!/usr/bin/python3

from sanitise import *

def generate_orchestration(namespace, prefix, orchestration):
    sane_prefix = sanitise_for_include_guard(prefix)
    filename = '{}Orchestration.cs'.format(prefix)
    with open(filename, 'w') as file:
        pass
