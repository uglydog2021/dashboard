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

      <!-- TaskUser -->
      <el-col :xs="24" :sm="12" :md="8" :lg="6">
        <el-form-item label="TaskUser" prop="TaskUser">
          <el-input
            v-model="queryForm.TaskUser"
            placeholder="TaskUser"
            style="width: 100%"
          />
        </el-form-item>
      </el-col>

      <!-- StartData -->
      <el-col :xs="24" :sm="12" :md="8" :lg="6">
        <el-form-item label="StartData" prop="StartData">
          <el-date-picker
            v-model="queryForm.StartData"
            type="date"
            placeholder="请选择日期"
            style="width: 100%"
          />
        </el-form-item>
      </el-col>

      <!-- EndData -->
      <el-col :xs="24" :sm="12" :md="8" :lg="6">
        <el-form-item label="EndData" prop="EndData">
          <el-date-picker
            v-model="queryForm.EndData"
            type="date"
            placeholder="请选择日期"
            style="width: 100%"
          />
        </el-form-item>
      </el-col>
    </el-row>

    <el-row :gutter="20">
      <!-- Status -->
      <el-col :xs="24" :sm="12" :md="8" :lg="6">
        <el-form-item label="Status" prop="Status">
          <el-select
            v-model="queryForm.Status"
            placeholder="Select"
            style="width: 100%"
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

      <!-- 操作按钮 -->
      <el-col :xs="24" :sm="12" :md="8" :lg="6">
        <el-form-item>
          <el-button type="primary" @click="handleQuery">検索</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success">导出数据</el-button>
        </el-form-item>
      </el-col>
    </el-row>
  </el-form>
    <!-- 查询结果表格 -->
    <el-table :data="tableData" border style="width: 100%">
      <el-table-column prop="id" label="ID" width="80" >
          <template #default="{ row }">
              <router-link :to="`/bill/${row.id}`" class="id-link">{{ row.id }}</router-link>
          </template>
      </el-table-column>
      <el-table-column prop="fileName" label="fileName" width="120" />
      <el-table-column prop="filePath" label="filePath" width="150" />
      <el-table-column prop="updateDate" label="updateDate" />
      <el-table-column prop="processMessage" label="processMessage" width="120" />
      <el-table-column prop="taskUser" label="taskUser" />
      <el-table-column prop="status" label="status" width="120" />
      <el-table-column prop="OrganizeID" label="OrganizeID" width="150" />
      <el-table-column prop="releaseMessage" label="releaseMessage" />
      <el-table-column prop="priority" label="priority" width="120" />
      <el-table-column prop="priority2" label="priority2" />
      <el-table-column prop="createDate" label="createDate" />
    </el-table>
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
  import { ref, onMounted, computed } from 'vue';
  import {storeToRefs} from "pinia"
  import { useSettingStore } from "@/stores/index";
  import { currentGET } from '@/api';
  import { statusOption } from './ScreenDesign2/component/consts';
  
  const settingStore = useSettingStore();
  const {dbArray} = storeToRefs(settingStore);
  settingStore.getDatebase();
  const dbs = computed(() => dbArray.value)
  const queryForm = ref({
      ConnectionName : '', 
      StartData : '', 
      EndData : '', 
      TaskUser : '', 
      Status : ''
  });
  
  const tableData = ref([]); // 表格数据
  const total = ref(0); // 总条数
  const pageSize = ref(30); // 每页显示条数
  const currentPage = ref(1); // 当前页码
  
  // 查询数据
  const fetchData = async () => {
    // 模拟查询条件过滤
    const filteredData = await currentGET('GetFilesManagement',{Index: currentPage.value, PageSize: pageSize.value, ...queryForm.value});//mockData(currentPage.value, pageSize.value, queryForm.value);
    tableData.value = filteredData;
    total.value = 1000; // 模拟总条数
  };

  // 初始化加载数据
  onMounted(() => {
    fetchData();
  });

  // 处理查询
  const handleQuery = () => {
    currentPage.value = 1; // 重置到第一页
    fetchData();
  };

  // 处理重置
  const handleReset = () => {
    queryForm.value = {
      ConnectionName : '',
      StartData : '',
      EndData : '',
      TaskUser : '',
      Status : '' };
    fetchData();
  };

  // 处理分页大小变化
  const handleSizeChange = (size: number) => {
    pageSize.value = size;
    fetchData();
  };

  // 处理页码变化
  const handlePageChange = (page : number) => {
    currentPage.value = page;
    fetchData();
  };
  
</script>
  
<style scoped>
  .query-page {
    padding: 20px;
  }
  .id-link {
    color: #409eff; /* 超链接颜色 */
    text-decoration: none; /* 去掉下划线 */
  }

  .id-link:hover {
    text-decoration: underline; /* 鼠标悬停时显示下划线 */
  }
  /* 确保表单元素宽度充满父容器 */
  .el-select,
  .el-input,
  .el-date-picker {
    width: 100%;
  }

  /* 响应式布局调整 */
  @media (max-width: 768px) {
    .el-form-item {
      margin-bottom: 16px; /* 在小屏幕上增加间距 */
    }
  }
</style>