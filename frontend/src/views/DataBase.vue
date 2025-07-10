<template>
  <div class="query-page">
    <!-- 查询条件 -->
    <el-form :model="queryForm" ref="queryFormRef" label-width="120px">
      <!-- <el-row :gutter="20"> -->
      <!-- DBName -->
      <!-- <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="DBName" prop="DBName">
            <el-input
              v-model="queryForm.dbName"
              placeholder="DBName"
              style="width: 100%"
            />
          </el-form-item>
        </el-col> -->

      <!-- StartData -->
      <!-- <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="Account" prop="Account">
            <el-input
              v-model="queryForm.account"
              placeholder="Account"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row> -->
      <div class="actionContainer">
        <el-button type="primary" @click="handleQuery">検索</el-button>
        <!-- <el-button type="warning" @click="handleEdit">編集</el-button> -->
        <!-- <el-button type="danger" @click="handleDelete">削除</el-button> -->
        <el-button type="success" @click="addNewData">新規追加</el-button>
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
      <el-table-column prop="dbName" label="dbName" />
    </el-table>
    <br />
    <!-- 分页 -->
    <!-- <el-pagination
      background
      layout="total, sizes, prev, pager, next, jumper"
      :total="total"
      :page-size="pageSize"
      :current-page="currentPage"
      @size-change="handleSizeChange"
      @current-change="handlePageChange"
    /> -->
    <el-dialog v-model="showDialog">
      <template #header>
        <h2 class="custom-title">{{ flag === 0 ? "新規追加" : "編集" }}</h2>
      </template>
      <el-form
        :model="editForm"
        ref="editFormRef"
        label-width="120px"
        class="form-container"
      >
        <!-- 第一行：输入框组 -->
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="dbName" prop="dbName">
              <el-input v-model="editForm.dbName" placeholder="请输入DBName" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="account" prop="account">
              <el-input
                v-model="editForm.account"
                placeholder="请输入Account"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="passWord" prop="passWord">
              <el-input
                v-model="editForm.passWord"
                placeholder="Password"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="showDialog = false">キャンセル</el-button>
          <el-button type="primary" @click="SaveDataBase"> 確定 </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { currentGET, currentPOST, currentPOST2 } from "../api";
import { ElMessage, ElMessageBox } from "element-plus";
import { DataBaseDTO, ResponseData } from "@/api/models";

const currentRow = ref<DataBaseDTO>();
const showDialog = ref(false);
const handleCurrentChange = (val: DataBaseDTO | undefined) => {
  currentRow.value = val!;
  console.log("handleCurrentChange", val);
};
const queryForm = ref<DataBaseDTO>({});
const editForm = ref<DataBaseDTO>({});

const tableData = ref<DataBaseDTO[]>([]); // 表格数据
const total = ref(0); // 总条数
const pageSize = ref(30); // 每页显示条数
const currentPage = ref(1); // 当前页码
const loading = ref(false); // 加载状态
const queryFormRef = ref(); // 表单引用
const editFormRef = ref(); // 表单引用
const flag = ref(0);
// const selectedConnName = ref('');
// 查询数据
const fetchData = async () => {
  loading.value = true;
  try {
    const filteredData: ResponseData<string[]> = await currentGET(
      "GetDataBase",
      {
        Index: currentPage.value,
        PageSize: pageSize.value,
        ...queryForm.value,
      }
    );
    tableData.value = filteredData.result.map((t) => {
      return {
        dbName: t,
      };
    });
    total.value = tableData.value.length; // 模拟总条数
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

const SaveDataBase = async () => {
  if (editForm.value) {
    if (flag.value === 0) {
      await currentPOST("addDBConnection", { ...editForm.value });
    } else {
      await currentPOST("UpdateDBConnection", { ...editForm.value });
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
  if (currentRow.value && currentRow.value.dbName != null) {
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
    ElMessageBox.confirm("Are you sure to close this dialog?")
      .then(async () => {
        if (currentRow.value && currentRow.value.dbName != null) {
          await currentPOST2("DeletePermission", {
            dbName: currentRow.value.dbName,
          });
          fetchData();
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
