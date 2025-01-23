# Build 자동화 관련 세팅 및 메모

## 1. Slack webhook shell script

https://slack.com/apps/A0F7XDUAZ-incoming-webhooks

```
#example
APPSEALING_STATUS="Off"
AUTO_UPLOAD_STATUS="Off"
if $USE_APPSEALING; then
	APPSEALING_STATUS="On"
fi
if $AUTO_UPLOAD; then
	AUTO_UPLOAD_STATUS="On"
fi
MESSAGE="[${JOB_NAME} 빌드 시작] 앱 실링 : ${APPSEALING_STATUS}, 자동 업로드 : ${AUTO_UPLOAD_STATUS}"

curl -X POST --data-urlencode "payload={\"channel\": \"#build_notification_dev\", \"username\": \"webhookbot\", \"text\": \"${MESSAGE}\", \"icon_emoji\": \":building_construction:\"}" https://hooks.slack.com/services/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
```
