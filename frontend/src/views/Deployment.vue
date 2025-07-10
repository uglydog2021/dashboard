<template>
  <div class="query-page">
    <!-- 查询条件 -->
    <el-form :model="queryForm" ref="queryFormRef" label-width="140px">
      <el-row :gutter="20">
        <!-- DeploymentId -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="部署ID" prop="OrganizationID">
            <el-input
              v-model="queryForm.OrganizationID"
              placeholder="部署ID"
              style="width: 100%"
              clearable
            />
          </el-form-item>
        </el-col>

        <!-- StartData -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="部署名字" prop="OrganizationName">
            <el-input
              v-model="queryForm.OrganizationName"
              placeholder="部署名字"
              style="width: 100%"
              clearable
            />
          </el-form-item>
        </el-col>

        <!-- 每批领取任务数 -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="Maker领取任务数" prop="MakerTaskCount">
            <el-input
              v-model="queryForm.MakerTaskCount"
              placeholder="Maker领取任务数"
              style="width: 100%"
              clearable
            />
          </el-form-item>
        </el-col>

        <!-- Publisher -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="Checker领取任务数" prop="CheckerTaskCount">
            <el-input
              v-model="queryForm.CheckerTaskCount"
              placeholder="Checker领取任务数"
              style="width: 100%"
              clearable
            />
          </el-form-item>
        </el-col>
      </el-row>
      <div class="actionContainer">
        <el-button type="primary" @click="handleQuery">查询</el-button>
        <el-button type="warning" @click="handleEdit">编辑</el-button>
        <el-button type="danger" @click="handleDelete">删除</el-button>
        <el-button type="success" @click="addNewData">添加</el-button>
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
        v-for="column in tableColumns"
        :key="column.prop"
        :prop="column.prop"
        :label="column.label"
        :width="column.width"
      >
        <!-- <template v-if="column.prop === 'fileName'" #default="{ row }">
          <router-link :to="{path: `/bill/${row.fileName}`,
                              query: {
                                ConnectionName: selectedConnName
          }}" class="id-link">{{ row.fileName }}</router-link>
        </template>
        <template v-else-if="column.prop === 'status'" #default="{ row }">
          {{ formatStatus(row.status) }}
        </template> -->
      </el-table-column>
    </el-table>
    <br />
    <!-- 分页 -->
    <el-pagination
      background
      layout="total, sizes, prev, pager, next, jumper"
      :total="total"
      v-model:page-size="pageSize"
      v-model:current-page="currentPage"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
    />
  </div>
  <el-dialog v-model="showDialog">
    <template #header>
      <h2 class="custom-title">{{ flag === 0 ? "新建" : "编辑" }}</h2>
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
          <el-form-item label="部署ID" prop="organizationID">
            <el-input
              v-model="editForm.organizationID"
              placeholder="请输入部署ID"
              :disabled="flag !== 0"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="20">
          <el-form-item label="部署名字" prop="DeploymentName">
            <el-input
              v-model="editForm.organizationName"
              placeholder="请输入部署名字"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="20">
          <el-form-item label="Maker领取任务数" prop="makerTaskCount">
            <el-input
              v-model="editForm.makerTaskCount"
              placeholder="请输入Maker领取任务数"
            />
          </el-form-item>
        </el-col>
      </el-row>
      <!-- 第二行：权限单选组 -->
      <el-row :gutter="20">
        <el-col :span="20">
          <el-form-item label="Checker领取任务数" prop="checkerTaskCount">
            <el-input
              v-model="editForm.checkerTaskCount"
              placeholder="请输入Checker领取任务数"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="showDialog = false">キャンセル</el-button>
        <el-button type="primary" @click="saveDeployment"> 確定 </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { useSettingStore } from "../stores/index";
import { currentGET, currentPOST, currentPOST2, currentPUT } from "../api";
import { ElMessage, ElMessageBox } from "element-plus";
import { OrganizationDO } from "@/api/models";

type QueryParam = {
  OrganizationID?: number;
  OrganizationName?: string;
  MakerTaskCount?: number;
  CheckerTaskCount?: number;
};
const flag = ref(0);
const currentRow = ref<OrganizationDO>();
const showDialog = ref(false);
const handleCurrentChange = (val: OrganizationDO | undefined) => {
  currentRow.value = val!;
  console.log("handleCurrentChange", val);
};
// 表格列配置
const tableColumns = [
  { prop: "organizationID", label: "部署ID", width: "160" },
  { prop: "organizationName", label: "部署名字" },
  { prop: "makerTaskCount", label: "Maker领取任务数" },
  { prop: "checkerTaskCount", label: "Checker领取任务数" },
];

// 数据初始化
const queryForm = ref<QueryParam>({});
const editForm = ref<OrganizationDO>({});

const tableData = ref([]); // 表格数据
const total = ref(0); // 总条数
const pageSize = ref(30); // 每页显示条数
const currentPage = ref(1); // 当前页码
const loading = ref(false); // 加载状态
const queryFormRef = ref(); // 表单引用
const editFormRef = ref(); // 表单引用
// const selectedConnName = ref('');
// 查询数据
const fetchData = async () => {
  loading.value = true;
  try {
    const filteredData = await currentGET("GetOrg", {
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
onMounted(() => {
  fetchData();
});
const saveDeployment = async () => {
  if (editForm.value) {
    if (flag.value === 0) {
      await currentPOST("AddOrganization", { ...editForm.value });
    } else {
      await currentPOST("UpdateOrganization", { ...editForm.value });
    }
    showDialog.value = false;
    await fetchData();
  }
};
const addNewData = () => {
  flag.value = 0;
  editForm.value = {};
  showDialog.value = true;
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
  if (currentRow.value && currentRow.value.organizationID != null) {
    flag.value = 1;
    editForm.value = currentRow.value;
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
        if (currentRow.value && currentRow.value.organizationID != null) {
          const filteredData = await currentPOST2("RemoveOrganization", {
            OrganizationId: currentRow.value.organizationID,
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

function currentDelete(arg0: string, arg1: { id: number }) {
  throw new Error("Function not implemented.");
}
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
