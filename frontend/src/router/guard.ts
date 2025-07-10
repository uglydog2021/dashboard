import type { RouteLocationNormalized, NavigationGuardNext } from 'vue-router'
import { StorageEnum } from '@/enums'
import {getLocalStorage} from "@/utils";
import usePermission from "@/hooks/use-permission"
import { useRouterCacheStore } from '@/stores';

const { permissions, setPermission } = usePermission();
export const token = (to: RouteLocationNormalized, from: RouteLocationNormalized, next: NavigationGuardNext) => {
  const routerCache = useRouterCacheStore();
  if (to.meta.shouldCacheFrom) {
    const shouldCacheFrom = to.meta.shouldCacheFrom as string
    routerCache.addCachedRoute(shouldCacheFrom.split(','))
  } else {
    if (!routerCache.checkCachedRoute(to.name as string)) {
      routerCache.routerName.clear()
    }
  }
  if (!to.meta.isPublic) {
    const token = getLocalStorage(StorageEnum.GB_TOKEN_STORE)
    if (!token) {
      next({ name: 'Login', replace: true })
      return
    }
    setPermission()
    if (to.meta.permission) {
      if (!permissions.value.WebPermission) {
        next({ name: 'Login', replace: true })
        return
      } else {
        if (!(to.meta.permission as number[]).includes(permissions.value.WebPermission)) {
          next({ name: 'Login', replace: true })
          return
        }
      }
    }
  }
  next()
}
