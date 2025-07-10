<template>
  <el-container>
    <!-- 左侧菜单 -->
    <el-aside width="200px">
      <div class="menu">
        <el-menu
          :default-active="activeMenu"
          class="el-menu-vertical-demo"
          @select="handleMenuSelect"
        >
          <el-menu-item index="/first-page">首页</el-menu-item>
          <!--大屏-->
          <el-menu-item index="/index">ダッシュボード</el-menu-item>
          <!--        &lt;!&ndash;单据主表查询&ndash;&gt;-->
          <el-menu-item
            v-if="
              permissions.WebPermission == 2 || permissions.WebPermission == 1
            "
            index="/bill"
            >注文一覧</el-menu-item
          >
          <!--        &lt;!&ndash;单据明细查看&ndash;&gt;-->
          <el-menu-item
            v-if="
              permissions.WebPermission == 2 || permissions.WebPermission == 1
            "
            index="/document"
            >注文詳細</el-menu-item
          >
          <!--        &lt;!&ndash;ActionHistory&ndash;&gt;-->
          <el-menu-item
            v-if="
              permissions.WebPermission == 2 || permissions.WebPermission == 1
            "
            index="/action-history"
            >アクション履歴</el-menu-item
          >
          <!--        &lt;!&ndash;返回件管理&ndash;&gt;-->
          <el-menu-item
            v-if="
              permissions.WebPermission == 2 || permissions.WebPermission == 1
            "
            index="/backData"
            >返品書類</el-menu-item
          >
          <el-menu-item v-if="permissions.WebPermission == 1" index="/attention"
            >Attention管理</el-menu-item
          >
          <!--权限配置页-->
          <el-menu-item
            v-if="permissions.WebPermission == 1"
            index="/permission"
            >権限設定</el-menu-item
          >
          <!--数据库维护-->
          <el-menu-item v-if="permissions.WebPermission == 1" index="/database"
            >DBメンテナンス</el-menu-item
          >
          <!--&lt;!&ndash;        <el-menu-item index="/user">用户表维护</el-menu-item>&ndash;&gt;-->
          <el-menu-item
            v-if="permissions.WebPermission == 1"
            index="/deployment"
            >部署管理</el-menu-item
          >
          <!--        &lt;!&ndash; <el-menu-item index="/deployment">休假管理</el-menu-item> &ndash;&gt;-->
          <el-menu-item v-if="permissions.WebPermission == 1" index="uploadFile"
            >上传休假文件</el-menu-item
          >
          <!--        <el-menu-item index="my">我的</el-menu-item>-->
          <!--        <el-sub-menu v-if="permissions.WebPermission == 1" index="2">-->
          <!--          <template #title>休假管理</template>-->
          <!--          <el-menu-item index="uploadFile">上传休假文件</el-menu-item>-->
          <!--          &lt;!&ndash; <el-menu-item index="leaveRequest">休假申请</el-menu-item> &ndash;&gt;-->
          <!--          <el-menu-item index="leaveRecord">休假记录</el-menu-item>-->
          <!--          <el-menu-item index="processManagement">审批管理</el-menu-item>-->
          <!--        </el-sub-menu>-->
        </el-menu>
      </div>
    </el-aside>
    <!-- 右侧内容 -->
    <el-container style="padding: 0">
      <el-header height="45px" style="padding: 0">
        <div class="user-info">
          <span>{{
            backends.find(
              (t) => t.value === getLocalStorage(StorageEnum.BACKEND_NAME)
            )?.label
          }}</span>
          <el-dropdown @command="handleCommand">
            <span class="el-dropdown-link">
              {{ permissions.UserName }}
            </span>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="permission"
                  >修改权限</el-dropdown-item
                >
                <el-dropdown-item command="logout">Log out</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </el-header>
      <!-- 路由视图 -->
      <el-main style="padding: 0"
        ><router-view v-slot="{ Component }">
          <keep-alive :include="Array.from(routerName)">
            <component :is="Component" />
          </keep-alive> </router-view
      ></el-main>
    </el-container>
  </el-container>
  <el-dialog v-model="showDialog">
    <el-form
      :model="editForm"
      ref="editFormRef"
      label-width="140px"
      class="form-container"
    >
      <!-- 第三行：输入权限单选组 -->
      <el-row :gutter="20">
        <el-col :span="20">
          <el-form-item label="データ入力者の権限" prop="data_Permission">
            <el-radio-group v-model="editForm.data_Permission">
              <el-radio :value="1">データ入力者</el-radio>
              <el-radio :value="2">チェッカー</el-radio>
              <el-radio :value="3">管理者</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item label="" prop="import_Permission">
            <el-checkbox
              v-model="editForm.import_Permission"
              :true-value="1"
              :false-value="0"
              label="データインポーター"
            />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="showDialog = false">キャンセル</el-button>
        <el-button type="primary" @click="savePermission"> 確定 </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useRouter, useRoute } from "vue-router";
import usePermission from "@/hooks/use-permission";
import { StorageEnum } from "@/enums";
import { getLocalStorage, clearLocalStorage } from "@/utils";
import { backends } from "@/views/ScreenDesign2/component/consts";
import { useRouterCacheStore } from "@/stores";
import { storeToRefs } from "pinia";
import { PermissionDTO, ResponseData } from "@/api/models";
import { currentGET, currentPOST } from "@/api";
import { ElMessage } from "element-plus";

const { permissions } = usePermission();
const routerCache = useRouterCacheStore();
const { routerName } = storeToRefs(routerCache);
const router = useRouter();
const route = useRoute();
const activeMenu = ref("/first-page");
const showDialog = ref(false);
const editForm = ref<PermissionDTO>({});
if (route.path && route.path.split("/")[1].trim().length > 0) {
  activeMenu.value = route.path;
}
// 缓存的视图列表
// const cachedViews = computed(() => {
//   const shouldCacheFrom = route.meta.shouldCacheFrom as string
//   if (typeof shouldCacheFrom !== 'undefined') {
//     return [shouldCacheFrom]
//   } else if (cachedViews.value?.includes(String(route.name))) {
//     return undefined
//   } else {
//     return []
//   }
// });
// 监听路由变化，更新菜单选中状态
watch(
  () => route.path,
  (newPath) => {
    activeMenu.value = newPath; //'/'+newPath.split('/')[1];
  }
);

// 处理菜单点击
const handleMenuSelect = (index: any) => {
  router.push(index);
};

// return {
//   activeMenu,
//   handleMenuSelect,
// };
const handleCommand = async (command: string | number | object) => {
  if (command === "logout") {
    clearLocalStorage(StorageEnum.GB_TOKEN_STORE);
    router.push({ name: "Login" });
  }
  if (command === "permission") {
    const filteredData: ResponseData<PermissionDTO[]> = await currentGET(
      "GetPermissionManagement",
      {
        guid: permissions.value.guid,
      }
    );
    if (filteredData.code === 0 && filteredData.result.length > 0) {
      editForm.value = {
        ...filteredData.result[0],
      };
      showDialog.value = true;
    } else {
      ElMessage.warning("信息不存在");
    }
  }
};
const savePermission = async () => {
  const res: ResponseData<boolean> = await currentPOST("UpdateUserMaster", {
    ...editForm.value,
  });
  if (res.code === 0) {
    ElMessage.success("修改成功");
  }
  showDialog.value = false;
};
// 动态计算需要缓存的组件名称列表
// const cachedComponents = computed(() => {
//   return router
//     .getRoutes()
//     .flatMap((t) => t.children || [])
//     .filter((route) => {
//       // 如果meta.keepAlive是函数，则传入当前路由和目标路由进行判断
//       if (typeof route.meta?.keepAlive === "function") {
//         return route.meta.keepAlive(
//           router.currentRoute.value,
//           router.currentRoute.value
//         );
//       }
//       return route.meta?.keepAlive === true;
//     })
//     .map((route) => route.name);
// });
</script>

<style>
.el-container {
  height: 100vh;
}
.el-aside {
  height: 100vh;
}
.app-container {
  display: flex;
}

.menu {
  width: 200px;
  background-color: #163172;
  height: 100vh;
}

.content {
  flex: 1;
  display: flex;
  flex-direction: column;
}
.user-info {
  padding: 10px 20px;
  background-color: #f0f0f0;
  border-bottom: 1px solid #ccc;
  display: flex;
  justify-content: flex-end;
}

.router-view {
  flex: 1;
  padding: 20px;
}
.el-dropdown {
  font-size: 16px;
  line-height: 24px;
  margin-left: 5px;
}
</style>
