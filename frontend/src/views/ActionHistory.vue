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
        <!-- ConnectionName -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="ConnectionName" prop="ConnectionName">
            <el-select
              v-model="queryForm.ConnectionName"
              placeholder="Select"
              style="width: 100%"
              clearable
              filterable
            >
              <el-option
                v-for="item in dbs"
                :key="item"
                :label="item"
                :value="item"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="12" :md="12" :lg="12">
          <el-form-item label="FileName" prop="FileName">
            <el-input
              v-model="queryForm.FileName"
              placeholder=""
              clearable
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <!-- TaskUser -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="TaskUser" prop="TaskUser">
            <el-input
              v-model="queryForm.TaskUser"
              placeholder="TaskUser"
              style="width: 100%"
              clearable
            />
          </el-form-item>
        </el-col>

        <!-- ActionStartDate -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="ActionStartDate" prop="ActionStartDate">
            <el-date-picker
              v-model="queryForm.ActionStartDate"
              type="date"
              value-format="YYYY-MM-DD"
              placeholder="请选择日期"
              style="width: 100%"
              clearable
            />
          </el-form-item>
        </el-col>

        <!-- ActionEndDate -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="ActionEndDate" prop="ActionEndDate">
            <el-date-picker
              v-model="queryForm.ActionEndDate"
              type="date"
              value-format="YYYY-MM-DD"
              placeholder="请选择日期"
              style="width: 100%"
              clearable
            />
          </el-form-item>
        </el-col>
        <!-- Status -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="Status" prop="Status">
            <el-select
              v-model="queryForm.Status"
              placeholder="Select"
              style="width: 100%"
              clearable
              filterable
            >
              <el-option
                v-for="item in statusOption"
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
        <el-button @click="handleReset">重置</el-button>
        <el-button type="success">导出数据</el-button>
      </div>
    </el-form>

    <!-- 查询结果表格 -->
    <el-table :data="tableData" border style="width: 100%" v-loading="loading">
      <el-table-column
        v-for="column in tableColumns"
        :key="column.prop"
        :prop="column.prop"
        :label="column.label"
        :width="column.width"
      >
        <template v-if="column.prop === 'fileName'" #default="{ row }">
          <router-link
            :to="{
              path: `/bill/${row.fileName}`,
              query: {
                ConnectionName: selectedConnName,
              },
            }"
            class="id-link"
            >{{ row.fileName }}</router-link
          >
        </template>
        <template v-else-if="column.prop === 'status'" #default="{ row }">
          {{ formatStatus(row.status) }}
        </template>
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
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from "vue";
import { storeToRefs } from "pinia";
import { useSettingStore } from "../stores/index";
import { currentGET } from "../api";
import { statusOption, formatStatus } from "./ScreenDesign2/component/consts";

// 表格列配置
const tableColumns = [
  { prop: "fileName", label: "fileName", width: "560" },
  // { prop: 'filePath', label: 'filePath', width: '150' },
  // { prop: 'updateDate', label: 'updateDate' },
  // { prop: 'processMessage', label: 'processMessage', width: '120' },
  { prop: "status", label: "status" },
  { prop: "taskUser", label: "taskUser" },
  // { prop: 'OrganizeID', label: 'OrganizeID', width: '120' },
  // { prop: 'releaseMessage', label: 'releaseMessage' },
  // { prop: 'priority', label: 'priority', width: '120' },
  // { prop: 'priority2', label: 'priority2' },
  { prop: "createDate", label: "createDate" },
];

// 表单验证规则
const validateEndDate = (rule: any, value: string, callback: Function) => {
  if (
    value &&
    queryForm.value.ActionEndDate &&
    queryForm.value.ActionStartDate &&
    queryForm.value.ActionEndDate < queryForm.value.ActionStartDate
  ) {
    callback(new Error("结束日期必须大于等于开始日期"));
  } else {
    callback();
  }
};

const rules = {
  ActionStartDate: [{ validator: validateEndDate, trigger: "change" }],
  ActionEndDate: [{ validator: validateEndDate, trigger: "change" }],
};

// 数据初始化
const settingStore = useSettingStore();
const { dbArray } = storeToRefs(settingStore);
settingStore.getDatebase();
const dbs = computed(() => dbArray.value);

const queryForm = ref<any>({});

const tableData = ref([]); // 表格数据
const total = ref(0); // 总条数
const pageSize = ref(30); // 每页显示条数
const currentPage = ref(1); // 当前页码
const loading = ref(false); // 加载状态
const queryFormRef = ref(); // 表单引用
const selectedConnName = ref();
// 查询数据
const fetchData = async () => {
  loading.value = true;
  try {
    const filteredData = await currentGET("GetActionHistoryList", {
      PageNumber: currentPage.value,
      PageSize: pageSize.value,
      ...queryForm.value,
    });
    tableData.value = filteredData.result;
    selectedConnName.value = queryForm.value.ConnectionName;
    total.value = filteredData.total; // 模拟总条数
  } catch (error) {
    console.error("数据加载失败", error);
  } finally {
    loading.value = false;
  }
};

// 初始化加载数据
// onMounted(() => {
//   fetchData();
// });

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

.actionContainer {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 20px;
}
</style>
