#!/bin/bash

if [ -z "$1" ]; then
    echo "No argument supplied"
    exit 1
elif [ "$1" == "hash" ]; then
  echo $(git rev-parse --short HEAD)
elif [ "$1" == "branch" ]; then
  echo $(git symbolic-ref --short -q HEAD)
fi
