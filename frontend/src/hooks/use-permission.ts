import { ref } from 'vue'
import {getLocalStorage} from "@/utils";
import {StorageEnum} from "@/enums";
import { jwtDecode } from 'jwt-decode'
type PermissionModel = {
    WebPermission?: number
    DataPermission?: number
    ImportPermission?: number
    UserName?: string
    guid?: string
}
const permissions = ref<PermissionModel>({})
const usePermission = () => {
    function setPermission() {
        try {
            const token = getLocalStorage(StorageEnum.GB_TOKEN_STORE)
            const payload: any = jwtDecode(token)
            const profile = JSON.parse(payload.profile)
            permissions.value.WebPermission = profile.WebPermission
            permissions.value.DataPermission = profile.DataPermission
            permissions.value.ImportPermission = profile.ImportPermission
            permissions.value.UserName = profile.UserName
            permissions.value.guid = profile.guid
        } catch (e) {
            console.error(e)
        }
    }
    return {
        permissions,
        setPermission,
    }
}

export default usePermission