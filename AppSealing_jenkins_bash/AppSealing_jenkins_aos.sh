#!/bin/bash
#appSealing tool 권한 부여.
chmod -R 750 "${WORKSPACE}/Tool/AppSealingCLITool/"
AS_JAR_PATH="${WORKSPACE}/Tool/AppSealingCLITool/sealing.jar"
AS_CONFIG_FILE_PATH="${WORKSPACE}/Tool/AppSealingCLITool/config.txt"

function CheckAppSealingFiles {
	echo "-- CheckAppSealingFiles $1 Start --"
    filePath=$1
    extension=$2
    fileName=$(basename "${filePath}")
    if [[ $fileName == *"sealed"* ]]; then
    	echo "Already Sealed File return."
    	return 0;
    fi
    dirPath=${filePath%/*}
    AS_SRC_APK=${filePath}
    AS_SEALED_APK="${dirPath}/${fileName%.*}_sealed.$extension"
    SIGNED_APK="${dirPath}/${fileName%.*}_sealed_signed.$extension"
    KEYSTORE="${WORKSPACE}/Client/daybrix.keystore"
    KEYSTORE_ALIAS="daybrix"
    KEYSTORE_PASS="Epdlqmflrtm6^^"
    
    # 이미 생성되어있는 sealing 파일 제거.
    
    if [ -f ${AS_SEALED_APK} ]; then
    	rm -r ${AS_SEALED_APK}
    fi
    if [ -f ${SIGNED_APK} ]; then
    	rm -r ${SIGNED_APK}
    fi
    
    # USE_APPSEALING이 TRUE 이면 sealing.
    if $USE_APPSEALING; then
      echo "[AppSealing] Start --"
      echo "fileName : ${fileName}"
      echo "dirPath : ${dirPath}"
      echo "AS_SRC_APK : ${AS_SRC_APK}"
      echo "AS_SEALED_APK : ${AS_SEALED_APK}"
      
      java -jar ${AS_JAR_PATH} -config ${AS_CONFIG_FILE_PATH} -srcapk ${AS_SRC_APK} -sealedapk ${AS_SEALED_APK}
      
      echo "[AppSealing] ${extension} Signing..."
      if [ ${extension} == "apk" ]; then
      	echo "[AppSealing] Zipalign APK --"
      	zipalign -p -f -v 4 ${AS_SEALED_APK} ${SIGNED_APK}
        echo "[AppSealing] Signing APK --"
        apksigner sign --ks ${KEYSTORE} --ks-key-alias ${KEYSTORE_ALIAS} --ks-pass pass:${KEYSTORE_PASS} ${SIGNED_APK}
        #APK 는 signed 만 남기고 sealed는 지운다.
        rm -r ${AS_SEALED_APK}
      else
      echo "[AppSealing] Signing AAB --"
      	jarsigner -verbose -sigalg SHA256withRSA -digestalg SHA-256 -keystore ${KEYSTORE} -storepass ${KEYSTORE_PASS} ${AS_SEALED_APK} ${KEYSTORE_ALIAS}
      fi
      echo ${KEYSTORE_PASS}
      
      echo "[AppSealing] End --"
    else
    	echo "Not Use AppSealing"
    fi
    echo "-- CheckAppSealingFiles $1 End --"
    
}

echo "-- AppSealing Check Start --"
# Check for APK files in the specified directory
APK_FILES="${WORKSPACE}/*.apk"
AAB_FILES="${WORKSPACE}/AppBundle/*.aab"
# Use globbing to check for files
shopt -s nullglob
apkFiles=( $APK_FILES )
aabFiles=( $AAB_FILES )

# APK 파일 Sealing Start.
if [ ${#apkFiles[@]} -eq 0 ]; then
echo "No APK files found in ${APK_FILES}."
else
	for filePath in $APK_FILES
    do
		CheckAppSealingFiles $filePath "apk"
    done
fi

# AAB 파일 Sealing Start.
if [ ${#aabFiles[@]} -eq 0 ]; then
echo "No AAB files found in ${AAB_FILES}."
else
  for filePath in $AAB_FILES
  do
  	CheckAppSealingFiles $filePath "aab"
  done
fi
echo "-- AppSealing Check Finish --"