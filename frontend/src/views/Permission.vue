<template>
  <div class="query-page">
    <!-- 查询条件 -->
    <el-form
      :model="queryForm"
      ref="queryFormRef"
      label-width="140px"
      class="form-container"
    >
      <!-- 第一行：输入框组 -->
      <el-row :gutter="20">
        <el-col :xs="24" :sm="12" :md="8">
          <el-form-item label="ユーザーID" prop="employeeId">
            <el-input
              v-model="queryForm.employeeId"
              placeholder="ユーザーID"
              clearable
            />
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="12" :md="8">
          <el-form-item label="ユーザー名" prop="user_Name">
            <el-input
              v-model="queryForm.user_Name"
              placeholder="ユーザー名"
              clearable
            />
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="12" :md="8">
          <el-form-item label="所属部署" prop="organizationID">
            <el-select
              v-model="queryForm.organizationID"
              style="width: 100%"
              placeholder="所属部署"
              clearable
            >
              <el-option
                v-for="item in orgList"
                :key="item.organizationID"
                :label="item.organizationName"
                :value="item.organizationID!"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="12" :md="8">
          <el-form-item label="業務タイプ" prop="businessType">
            <el-select
              v-model="queryForm.businessType"
              style="width: 100%"
              placeholder="業務タイプ"
              clearable
            >
              <el-option
                v-for="item in businessTypeOption"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <div class="actionContainer">
        <el-button type="primary" @click="handleQuery">検索</el-button>
        <el-button type="warning" @click="handleEdit">編集</el-button>
        <el-button type="danger" @click="handleDelete">削除</el-button>
        <el-button type="success" @click="handleAdd">新規追加</el-button>
      </div>
    </el-form>

    <!-- 查询结果表格 -->
    <el-table
      highlight-current-row
      style="width: 100%"
      @current-change="handleCurrentChange"
      :data="tableData"
      border
      v-loading="loading"
    >
      <el-table-column
        prop="employeeId"
        label="ユーザーID"
        width="140"
      ></el-table-column>
      <el-table-column
        prop="user_Name"
        label="ユーザー名"
        width="120"
      ></el-table-column>
      <el-table-column
        prop="organizationID"
        label="所属部署"
        :formatter="orgFmt"
      ></el-table-column>
      <el-table-column
        prop="user_Email"
        label="Email"
        show-overflow-tooltip
      ></el-table-column>
      <el-table-column
        prop="web_Permission"
        label="Web権限"
        :formatter="webPermissionFmt"
        width="120"
      ></el-table-column>
      <el-table-column
        prop="data_Permission"
        label="データ入力権限"
        :formatter="dataPermissionFmt"
        width="130"
      ></el-table-column>
      <el-table-column
        prop="import_Permission"
        label="データインポーター"
        :formatter="ynFmt"
        width="155"
      ></el-table-column>
      <el-table-column
        prop="businessTypeText"
        label="業務タイプ"
        width="190"
      ></el-table-column>
      <el-table-column
        prop="createTime"
        label="アカウント作成時間"
        :formatter="dateTimeFmt"
        width="180"
      ></el-table-column>
    </el-table>
    <br />
    <!-- 分页 -->
    <el-pagination
      background
      layout="total, sizes, prev, pager, next, jumper"
      :total="total"
      :page-size="pageSize"
      :current-page="currentPage"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
    />
    <el-dialog v-model="showDialog">
      <template #header>
        <h2 class="custom-title">{{ flag === 0 ? "新規追加" : "編集" }}</h2>
      </template>
      <el-form
        :model="editForm"
        ref="editFormRef"
        label-width="140px"
        class="form-container"
      >
        <!-- 第一行：输入框组 -->
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="guid" prop="guid">
              <el-input v-model="editForm.guid" placeholder="guid" :disabled="flag !== 0" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="ユーザーID" prop="employeeId">
              <el-input
                v-model="editForm.employeeId"
                placeholder="ユーザーID"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="ユーザー名" prop="user_Name">
              <el-input v-model="editForm.user_Name" placeholder="ユーザー名" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="所属部署" prop="organizationID">
              <el-select
                v-model="editForm.organizationID"
                style="width: 100%"
                placeholder="所属部署"
              >
                <el-option
                  v-for="item in orgList"
                  :key="item.organizationID"
                  :label="item.organizationName"
                  :value="item.organizationID!"
                />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row v-if="flag === 0" :gutter="20">
          <el-col :span="20">
            <el-form-item label="パスワード" prop="password">
              <el-input v-model="editForm.password" placeholder="パスワード" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="Email" prop="user_Email">
              <el-input v-model="editForm.user_Email" placeholder="Email" />
            </el-form-item>
          </el-col>
        </el-row>
        <!-- 第二行：权限单选组 -->
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="Webページの権限" prop="web_Permission">
              <el-radio-group v-model="editForm.web_Permission">
                <el-radio :value="1">管理者</el-radio>
                <el-radio :value="2"> 一般ユーザー</el-radio>
                <el-radio :value="3">閲覧者</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>

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
        <!-- 业务类型 -->
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="業務タイプ" prop="businessType">
              <el-select
                v-model="editForm.businessType"
                style="width: 100%"
                placeholder="業務タイプ"
              >
                <el-option
                  v-for="item in businessTypeOption"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
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
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { currentGET, currentPOST, currentPOST2 } from "../api";
import { businessTypeOption } from "./ScreenDesign2/component/consts";
import { dateTimeFmt, ynFmt } from "@/utils";

import { ElMessage, ElMessageBox } from "element-plus";
import { OrganizationDO, PermissionDTO } from "@/api/models";

const currentRow = ref<PermissionDTO>();
const orgList = ref<OrganizationDO[]>([]);
const showDialog = ref(false);
const handleCurrentChange = (val: PermissionDTO | undefined) => {
  currentRow.value = val!;
  console.log("handleCurrentChange", val);
};

const webPermissions: Record<number, string> = {
  1: "管理者",
  2: "一般ユーザー",
  3: "閲覧者",
};

const dataPermissions: Record<number, string> = {
  1: "データ入力者",
  2: "チェッカー",
  3: "管理者",
};
const getOrg = async () => {
  const res = await currentGET("GetOrg", {
    PageSize: 9999,
  });
  orgList.value = res.result;
};
// 数据初始化
const queryForm = ref<PermissionDTO>({});
const editForm = ref<PermissionDTO>({});
const tableData = ref([]); // 表格数据
const total = ref(0); // 总条数
const pageSize = ref(10); // 每页显示条数
const currentPage = ref(1); // 当前页码
const flag = ref(0); // 当前页码
const loading = ref(false); // 加载状态
const queryFormRef = ref(); // 表单引用
const editFormRef = ref(); // 表单引用
// 查询数据
const fetchData = async () => {
  loading.value = true;
  try {
    const filteredData = await currentGET("GetPermissionManagement", {
      PageNumber: currentPage.value,
      PageSize: pageSize.value,
      ...queryForm.value,
    });
    tableData.value = filteredData.result;
    total.value = filteredData.total; // 模拟总条数
  } catch (error) {
    console.error("数据加载失败", error);
  } finally {
    loading.value = false;
  }
};

// 初始化加载数据
onMounted(async () => {
  await getOrg();
  fetchData();
});
const savePermission = async () => {
  showDialog.value = false;
  if (editForm.value) {
    if (flag.value === 0) {
      await currentPOST("CreateUserMaster", { ...editForm.value });
    } else {
      await currentPOST("UpdateUserMaster", { ...editForm.value });
    }
    await fetchData();
  }
};
// 处理查询
const handleQuery = () => {
  queryFormRef.value.validate((valid: boolean) => {
    if (valid) {
      currentPage.value = 1; // 重置到第一页
      fetchData();
    } else {
      console.log("表单验证失败");
    }
  });
};
const handleEdit = () => {
  if (currentRow.value && currentRow.value.guid != null) {
    flag.value = 1;
    editForm.value = { ...currentRow.value, password: undefined };
    showDialog.value = true;
  } else {
    ElMessage({
      message: "尚未选中编辑项！",
      type: "warning",
    });
  }
};

const handleDelete = () => {
  if (currentRow.value) {
    ElMessageBox.confirm("确认删除?")
      .then(async () => {
        if (currentRow.value && currentRow.value.guid != null) {
          await currentPOST2("RemoveUserMaster", {
            guid: currentRow.value.guid,
          });
          await fetchData();
        }
      })
      .catch(() => {
        console.error("delete permission data error");
      });
  } else {
    ElMessage({
      message: "尚未选中编辑项！",
      type: "warning",
    });
  }
};

// 处理重置
const handleReset = () => {
  queryFormRef.value.resetFields(); // 重置表单数据和验证状态
  fetchData();
};

// 处理分页大小变化
const handleSizeChange = (size: number) => {
  pageSize.value = size;
  fetchData();
};

// 处理页码变化
const handlePageChange = (page: number) => {
  currentPage.value = page;
  fetchData();
};

const handleAdd = () => {
  showDialog.value = true;
  flag.value = 0;
  editForm.value = {};
};
const orgFmt = (_row: any, _column: any, val: any) => {
  if (val) {
    return (
      orgList.value.find((t) => t.organizationID === val)?.organizationName ??
      ""
    );
  }
  return "";
};
const webPermissionFmt = (_row: any, _column: any, val: any) => {
  if (val) {
    return webPermissions[val] ?? "";
  }
  return "";
};
const dataPermissionFmt = (_row: any, _column: any, val: any) => {
  if (val) {
    return dataPermissions[val] ?? "";
  }
  return "";
};
</script>

<style scoped>
.query-page {
  padding: 20px;
}

.id-link {
  color: #409eff;
  text-decoration: none;
}

.id-link:hover {
  text-decoration: underline;
}

/* 统一输入框宽度 */
.el-input {
  width: 100%;
}

/* 按钮组右对齐样式 */
.action-buttons {
  display: flex;
  justify-content: flex-end;
  gap: 12px; /* 现代浏览器间距方案 */
  margin-top: 16px; /* 添加顶部间距 */
}

/* 兼容旧浏览器的备选方案 */
.action-buttons > * {
  margin-left: 12px;
}

.actionContainer {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 20px;
}
</style>
