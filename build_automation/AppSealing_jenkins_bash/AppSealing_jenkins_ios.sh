#!/bin/bash
#AppSealing
if [ ${USE_APPSEALING} == "TRUE" ]; then
	echo "-- AppSealing Start --"
	IPA_PATH="${WORKSPACE}/adhoc-build/build/*.ipa"
    TOOL_PATH="${WORKSPACE}/adhoc/Libraries/AppSealingSDK/Tools/*"
    GENERATE_HASH_PATH="${TOOL_PATH}/generate_hash"
    #Tool 권한 수정
    chmod +x ${TOOL_PATH}
    ${GENERATE_HASH_PATH} ${IPA_PATH}
    echo "-- AppSealing Done --"
fi