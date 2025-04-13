#!/bin/bash

rsync --exclude='install.sh' * ../../.git/hooks/
echo 'Local git hooks updated successfully'
