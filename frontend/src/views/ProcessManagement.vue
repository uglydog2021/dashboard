<template>
  <div class="query-page">
    <el-table ref="multipleTableRef"
    :data="tableData" v-loading="loading"
    row-key="id"
    style="width: 100%"
    @selection-change="handleSelectionChange"
  >
    <el-table-column type="selection" width="55" />  
    <el-table-column
        v-for="column in tableColumns"
        :key="column.prop"
        :prop="column.prop"
        :label="column.label"
        :width="column.width"
      >
        <template v-if="column.prop === 'type'" #default="{ row }">
          {{ formatStatus(row.type) }}
        </template>
      </el-table-column>
    </el-table>
    <br/>
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
    <div class="actionContainer">
        <el-button type="primary" @click="handleApproval">批准</el-button>
        <!-- <el-button @click="handleReset">重置</el-button> -->
        <el-button type="danger" @click="handleReject">拒绝</el-button>
        <!-- <el-button @click="handleReset">重置</el-button>
        <el-button type="success">导出数据</el-button> -->
      </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { currentGET } from '@/api';
import { typeOption } from './ScreenDesign2/component/consts';
import { ElMessage, ElMessageBox, TableInstance } from 'element-plus';
const multipleTableRef = ref<TableInstance>()
const multipleSelection = ref<LeaveRecordDTO[]>([])
const handleSelectionChange = (val: LeaveRecordDTO[]) => {
  multipleSelection.value = val
}

export interface LeaveRecordDTO {
  id: number
  taskUser: string
  startDate: string
  endDate: string
  department: string
  type: string
}
// 表格列配置
const tableColumns = [
  { prop: 'id', label: 'id'},
  { prop: 'taskUser', label: 'taskUser'},
  { prop: 'department', label: 'department' },
  { prop: 'interval', label: 'interval' },
  { prop: 'type', label: 'type', width: '260' }
];

// 根据状态值获取对应的文本描述
const formatStatus = (status: string) => {
  const statusItem = typeOption.find((item) => item.value === status);
  return statusItem ? statusItem.label : '未知状态';
};

const tableData = ref([]); // 表格数据
const total = ref(0); // 总条数
const pageSize = ref(30); // 每页显示条数
const currentPage = ref(1); // 当前页码
const loading = ref(false); // 加载状态
const queryFormRef = ref(); // 表单引用
// 查询数据
const fetchData = async () => {
  loading.value = true;
  try {
    const filteredData = await currentGET('GetLeaveRecord', {
      Index: currentPage.value,
      PageSize: pageSize.value
    });
    tableData.value = filteredData;
    // console.log("queryForm.conn",queryForm.value.ConnectionName)
    // selectedConnName.value = queryForm.value.ConnectionName;
    total.value = 1000; // 模拟总条数
  } catch (error) {
    console.error('数据加载失败', error);
  } finally {
    loading.value = false;
  }
};

// 初始化加载数据
onMounted(() => {
  fetchData();
});

// 处理重置
const handleApproval = () => {
  if(multipleSelection.value.length > 0){
    console.log(multipleSelection.value,"multipleSelection.value");
  }else{
    ElMessage({
      message: '尚未选中编辑项！',
      type: 'warning',
    })
  }
  
  // queryFormRef.value.resetFields(); // 重置表单数据和验证状态
  // fetchData();
};

const handleReject = () => {
  if(multipleSelection.value.length > 0){
    console.log(multipleSelection.value,"multipleSelection.value");
  }else{
    ElMessage({
      message: '尚未选中编辑项！',
      type: 'warning',
    })
  }
}

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