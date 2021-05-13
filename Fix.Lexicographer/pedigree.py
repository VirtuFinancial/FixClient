#!/usr/bin/python3

def format_pedigree(pedigree):
    initialisers = []
    if pedigree.added:
        initialisers.append('Added = "{}"'.format(pedigree.added))
    if pedigree.addedEP:
        initialisers.append('AddedEP = "{}"'.format(pedigree.addedEP))
    if pedigree.updated:
        initialisers.append('Updated = "{}"'.format(pedigree.updated))
    if pedigree.updatedEP:
        initialisers.append('UpdatedEP = "{}"'.format(pedigree.updatedEP))
    if pedigree.deprecated:
        initialisers.append('Deprecated = "{}"'.format(pedigree.deprecated))
    if pedigree.deprecatedEP:
        initialisers.append('DeprecatedEP = "{}"'.format(pedigree.deprecatedEP))
    return 'new Pedigree {{{}}}'.format(', '.join(initialisers))