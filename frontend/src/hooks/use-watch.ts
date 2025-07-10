import {watchEffect} from "vue";
import {storeToRefs} from "pinia"
import {currentGET} from "@/api"
import {useSettingStore} from "@/stores"
//
const useWatchDateRange = (apiKey: string | string[], setEcharts: (r: any) => void, param?: object) => {
    const settingStore = useSettingStore()
    const {dateRange} = storeToRefs(settingStore);
    watchEffect(async () => {
        if (dateRange.value
            && dateRange.value.group && dateRange.value.team && dateRange.value.interval > -1) {
            let initParam = {
                // StartData: dateRange.value.dateArr[0],
                // EndData: dateRange.value.dateArr[1],
                // databaseName: dateRange.value.databaseName,
                group: dateRange.value.group,
                team: dateRange.value.team,
            }
            let paramObj: any = {
                // ConnectionName: dateRange.value.team,
                OrganizationID: dateRange.value.group
            }
            if (apiKey === 'GetMakerTotalCount') {
                paramObj.Type = dateRange.value.peroid
                console.log("GetMaker------------- : ", paramObj)
            }
            if (apiKey === 'GetCheckerTotalCount') {
                paramObj.Type = dateRange.value.cycle
                console.log("GetChecker================ : ", paramObj)
            }

            try {
                if (isStringArray(apiKey)) {
                    const res = await Promise.all(apiKey.map((item: any) => currentGET(item, paramObj)))
                    setEcharts(res)
                } else {
                    const res = await currentGET(apiKey as string, paramObj)
                    setEcharts(res)
                }
            } catch (e) {
                console.error(e)
            }
        }
    })
}

function isStringArray(value: unknown): value is string[] {
    return Array.isArray(value) && value.every(item => typeof item === 'string');
}

export default useWatchDateRange
