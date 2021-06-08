#!/usr/bin/python3

import os
import sys
import argparse

# This package is subtree merged
sys.path.append(os.path.join(os.path.dirname(os.path.abspath(__file__)), '..', 'fixorchestra'))
from fixorchestra.orchestration import *

from orchestration_data_types_generator import *
from orchestration_fields_generator import *
from orchestration_messages_generator import *
from orchestration_generator import *


def inject_message_copy(orchestration, sourceMsgType, destinationMsgType, destinationName):
    source = orchestration.messages_by_msg_type[sourceMsgType]
    ids = [int(x) for x in list(orchestration.messages.keys())]
    ids.sort()
    next_id = max(ids) + 1
    destination = Message(next_id, destinationName, destinationMsgType, source.category, source.synopsis, source.pedigree, source.references)
    orchestration.messages[next_id] = destination

def inject_kodiak_messages(orchestration):
    if orchestration.version != "FIX.4.0":
        print("Skipping Kodiak for non applicable FIX version {}".format(orchestration.version))
        return
    print('Injecting Kodiak messages into {} orchestration'.format(orchestration.version))
    inject_message_copy(orchestration, "E", "UWO", "KodiakWaveOrder")
    inject_message_copy(orchestration, "G", "UWOCorrR", "KodiakWaveOrderCorrectionRequest")
    inject_message_copy(orchestration, "F", "UWOCanR", "KodiakWaveOrderCancelRequest")
    inject_message_copy(orchestration, "H", "UWOSR", "KodiakWaveOrderStatusRequest")
    inject_message_copy(orchestration, "J", "UWALLOC", "KodiakWaveAllocation")


if __name__ == '__main__':

    parser = argparse.ArgumentParser()
    parser.add_argument('--namespace', required=True, help='The namespace to generate code in')
    parser.add_argument('--prefix', required=True, help='The prefix for the generated filenames')
    parser.add_argument('--orchestration', required=True, help='The orchestration filename to generate code for')

    args = parser.parse_args()

    orchestration = Orchestration(args.orchestration)

    inject_kodiak_messages(orchestration)

    generate_orchestration_data_types(args.namespace, args.prefix, orchestration)
    generate_orchestration_fields(args.namespace, args.prefix, orchestration)
    generate_orchestration_messages(args.namespace, args.prefix, orchestration)
    generate_orchestration(args.namespace, args.prefix, orchestration)

