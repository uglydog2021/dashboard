<template>
  <div class="query-page">
    <!-- 查询条件 -->
    <el-form :model="queryForm" ref="queryFormRef" label-width="120px">
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
      </el-row>
      <div class="actionContainer">
        <el-button type="primary" @click="handleQuery">検索</el-button>
        <el-button @click="handleReset">重置</el-button>
        <el-button @click="exportData" type="success">导出数据</el-button>
      </div>
    </el-form>
    <!-- 查询结果表格 -->
    <el-table :data="tableData" border style="width: 100%" v-loading="loading">
      <el-table-column
        prop="fileName"
        label="fileName"
        width="510px"
      ></el-table-column>
      <el-table-column
        prop="lastUpdateFlag"
        label="lastUpdateFlag"
        width="130px"
      ></el-table-column>
      <el-table-column
        prop="lastUpdateUser"
        label="lastUpdateUser"
        width="140px"
      ></el-table-column>
      <el-table-column
        prop="createTime"
        label="createTime"
        width="180px"
        :formatter="dateTimeFmt"
      ></el-table-column>
      <el-table-column
        prop="updateTime"
        label="updateTime"
        width="180px"
        :formatter="dateTimeFmt"
      ></el-table-column>
      <el-table-column align="center" label="FileContent">
        <template #default="{ row }">
          <el-icon @click="showFileContent(row.FileContent)"
            ><Document
          /></el-icon>
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
    <el-dialog v-model="dialogVisible" title="FileContent" width="100%">
      <v-ace-editor
        v-model:value="content"
        lang="xml"
        theme="chrome"
        :min-lines="1"
        :max-lines="35"
      />
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import { storeToRefs } from "pinia";
import { useSettingStore } from "@/stores/index";
import { currentFILEBody, currentGET } from "@/api";
import { dateTimeFmt, saveFile } from "@/utils";
import { Document } from "@element-plus/icons-vue";
import { VAceEditor } from "vue3-ace-editor";
import { formatXml } from "@/utils/xmlFmt";
type QueryParam = {
  ConnectionName?: string;
  FileName?: string;
};
// 数据初始化
const dialogVisible = ref(false);
const content = ref("");
const settingStore = useSettingStore();
const { dbArray } = storeToRefs(settingStore);
settingStore.getDatebase();
const dbs = computed(() => dbArray.value);

const queryForm = ref<QueryParam>({});

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
    const filteredData = await currentGET("GetOrderInfo", {
      PageNumber: currentPage.value,
      PageSize: pageSize.value,
      ...queryForm.value,
    });
    if (filteredData.code === 0) {
      tableData.value = filteredData.result;
      selectedConnName.value = queryForm.value.ConnectionName;
      total.value = filteredData.total; // 模拟总条数
    }
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

const exportData = async () => {
  const data: any = await currentFILEBody("OrderInfoExportExcel", {
    ...queryForm.value,
  });
  saveFile(data);
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

const showFileContent = (FileContent: string) => {
  dialogVisible.value = true;
  content.value = formatXml(FileContent);
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
