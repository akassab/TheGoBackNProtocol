#!/bin/bash

# give permissions
chmod u+x ./A2Receiver/linux_executable/A2Receiver

# run receiver
source ./A2Receiver/linux_executable/A2Receiver $1 $2 $3 $4
