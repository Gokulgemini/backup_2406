#!/bin/bash

# Command line options.
DATABASES="--all-databases"
DESTDIR=""
EXTRAOPTS=""
GZIP=`which gzip 2> /dev/null`
MYSQLDUMP=`which mysqldump 2> /dev/null`
MYSQLUSER="root"
SHOW_HELP=0

while getopts "d:g:hm:D:" opt;
do
  case ${opt} in
    d ) # Output directory. Must be passed.
      DESTDIR="$OPTARG"
      ;;
    e ) # Custom options to pass along to mysqldump.
      EXTRAOPTS="$OPTARG"
      ;;
    g ) # Path to gzip.
      GZIP="$OPTARG"
      ;;
    h ) # Should command line help.
      SHOW_HELP=1
      ;;
    m ) # Path to mysqldump.
      MYSQLDUMP="$OPTARG"
      ;;
    u ) # MySQL user.
      MYSQLUSER="$OPTARG"
      ;;
    D ) # Databases to backup
      DATABASES="$OPTARG"
      ;;
  esac
done

if [ $SHOW_HELP -gt 0 ] || [ "$DESTDIR" == "" ]
then
	echo "$0:"
	echo "    -d Destination directory, required, no default."
	echo "    -e (extra command line options for mysqldump)"
	echo "    -g (path to gzip, default is [$GZIP])"
	echo "    -h (show help)"
	echo "    -m (path to mysqldump, default is [$MYSQLDUMP])"
	echo "    -u (MySQL user, default is [$MYSQLUSER])"
	echo "    -D (Databases to backup, default is [$DATABASES])"
	exit 255
fi

if [ "$GZIP" == "" ]
then
	echo "ERROR: Unable to execute gzip, location not defined."
	exit 255
fi

if [ "$MYSQLDUMP" == "" ]
then
	echo "ERROR: Unable to execute mysqldump, location not defined."
	exit 255
fi

TIMESTAMP=`date +%s`
DUMPFILE="$DESTDIR/$TIMESTAMP.mysqldump.gz"

STANDARDOPTS="--skip-lock-tables --single-transaction --flush-logs --hex-blob"

$MYSQLDUMP $STANDARDOPTS -u $MYSQLUSER $EXTRAOPTS $DATABASES | $GZIP > $DUMPFILE
