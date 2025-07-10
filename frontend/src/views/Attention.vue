<template>
  <div class="query-page">
    <!-- 查询条件 -->
    <el-form
      :model="queryForm"
      ref="queryFormRef"
      label-width="120px"
      :rules="rules"
    >
      <el-row :gutter="20">
        <!-- Organization -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="OrganizationName" prop="OrganizationName">
            <el-input
              v-model="queryForm.OrganizationName"
              placeholder="OrganizationName"
              style="width: 100%"
              clearable
            />
          </el-form-item>
        </el-col>

        <!-- StartData -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="StartDate" prop="Start_Time">
            <el-date-picker
              v-model="queryForm.Start_Time"
              type="date"
              value-format="YYYY-MM-DD"
              placeholder="请选择日期"
              style="width: 100%"
              clearable
            />
          </el-form-item>
        </el-col>

        <!-- EndData -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="EndDate" prop="End_Time">
            <el-date-picker
              v-model="queryForm.End_Time"
              type="date"
              value-format="YYYY-MM-DD"
              placeholder="请选择日期"
              style="width: 100%"
              clearable
            />
          </el-form-item>
        </el-col>

        <!-- Publisher -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="Publisher" prop="Publisher">
            <el-input
              v-model="queryForm.User_Id"
              placeholder="Publisher"
              style="width: 100%"
              clearable
            />
          </el-form-item>
        </el-col>
      </el-row>
      <div class="actionContainer">
        <el-button type="primary" @click="handleQuery">検索</el-button>
        <el-button type="warning" @click="handleEdit">編集</el-button>
        <el-button type="danger" @click="handleDelete">削除</el-button>
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
      <el-table-column prop="organizationName" label="組織名" width="130">
      </el-table-column>
      <el-table-column
        prop="start_Time"
        label="開始時間"
        :formatter="dateTimeFmt"
        width="180px"
      >
      </el-table-column>
      <el-table-column
        prop="end_Time"
        label="終了時間"
        :formatter="dateTimeFmt"
        width="180px"
      >
      </el-table-column>
      <el-table-column prop="subject" label="subject"> </el-table-column>
      <el-table-column prop="message" label="message" show-overflow-tooltip>
      </el-table-column>
      <el-table-column prop="user_Id" label="発行者" width="140px">
      </el-table-column>
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
            <el-form-item label="組織名" prop="organizationId">
              <el-select
                v-model="editForm.organizationId"
                style="width: 100%"
                placeholder="組織名"
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
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="Attention 開始時間" prop="start_Time">
              <el-date-picker
                v-model="editForm.start_Time"
                type="date"
                placeholder="Attention 開始時間"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="Attention 終了時間" prop="end_Time">
              <el-date-picker
                v-model="editForm.end_Time"
                type="date"
                placeholder="Attention 終了時間"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <!-- 第二行：权限单选组 -->
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="Attention 発行者" prop="user_Id">
              <el-input
                v-model="editForm.user_Id"
                placeholder="Attention 発行者"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="subject" prop="subject">
              <el-input v-model="editForm.subject" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="20">
            <el-form-item label="message" prop="message">
              <el-input v-model="editForm.message" type="textarea" :rows="4" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="showDialog = false">キャンセル</el-button>
          <el-button type="primary" @click="saveAttention"> 確定 </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { currentGET, currentPOST, currentPOST2 } from "../api";
import { ElMessage, ElMessageBox } from "element-plus";
import { dateTimeFmt } from "@/utils";
import { AttentionDO, OrganizationDO } from "@/api/models";

type QueryParam = {
  OrganizationName?: string;
  Start_Time?: string;
  End_Time?: string;
  User_Id?: string;
};

// 表单验证规则
const validateEndDate = (rule: any, value: string, callback: Function) => {
  if (
    value &&
    queryForm.value.End_Time &&
    queryForm.value.Start_Time &&
    queryForm.value.End_Time < queryForm.value.Start_Time
  ) {
    callback(new Error("结束日期必须大于等于开始日期"));
  } else {
    callback();
  }
};

const rules = {
  Start_Time: [{ validator: validateEndDate, trigger: "change" }],
  End_Time: [{ validator: validateEndDate, trigger: "change" }],
};

const flag = ref(0);
const currentRow = ref<AttentionDO>();
const showDialog = ref(false);
const handleCurrentChange = (val: AttentionDO | undefined) => {
  currentRow.value = val!;
  console.log("handleCurrentChange", val);
};

// 数据初始化
const queryForm = ref<QueryParam>({});

const editForm = ref<AttentionDO>({});

const tableData = ref([]); // 表格数据
const total = ref(0); // 总条数
const pageSize = ref(30); // 每页显示条数
const currentPage = ref(1); // 当前页码
const loading = ref(false); // 加载状态
const queryFormRef = ref(); // 表单引用
const editFormRef = ref(); // 表单引用
const orgList = ref<OrganizationDO[]>([]);
// const selectedConnName = ref('');
// 查询数据
const fetchData = async () => {
  loading.value = true;
  try {
    const filteredData = await currentGET("GetAttention", {
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
const getOrg = async () => {
  const res = await currentGET("GetOrg", {
    PageSize: 9999,
  });
  orgList.value = res.result;
};
const saveAttention = async () => {
  if (editForm.value) {
    if (flag.value === 0) {
      await currentPOST("AddAttention", {
        ...editForm.value,
        organizationName: orgList.value.find(
          (t) => t.organizationID === editForm.value.organizationId
        )?.organizationName,
      });
    } else {
      await currentPOST("UpdateAttention", {
        ...editForm.value,
        organizationName: orgList.value.find(
          (t) => t.organizationID === editForm.value.organizationId
        )?.organizationName,
      });
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
  if (currentRow.value && currentRow.value.attention_Id != null) {
    flag.value = 1;
    editForm.value = { ...currentRow.value };
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
        if (currentRow.value && currentRow.value.attention_Id != null) {
          await currentPOST2("RemoveAttention", {
            AttentionId: currentRow.value.attention_Id,
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
