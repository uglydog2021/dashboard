import { defineStore } from "pinia";
import { ref } from "vue";

export const useRouterCacheStore = defineStore("routerCache", () => {
  const routerName = ref(new Set<string>());
  function addCachedRoute(name: string[]) {
    routerName.value.clear()
    name.forEach(t => routerName.value.add(t))
  }
  function removeCachedRoute(name: string) {
    routerName.value.delete(name);
  }
  function checkCachedRoute(name: string) {
    return routerName.value.has(name);
  }
  return {
    routerName,
    addCachedRoute,
    removeCachedRoute,
    checkCachedRoute
  };
});
