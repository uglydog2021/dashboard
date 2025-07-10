<template>
    <div class="query-page">
      <!-- 查询条件 -->
      <el-form :model="queryForm" ref="queryFormRef" inline>
        <el-form-item label="姓名" prop="name">
          <el-input v-model="queryForm.name" placeholder="请输入姓名" />
        </el-form-item>
        <el-form-item label="电话" prop="phone">
          <el-input v-model="queryForm.phone" placeholder="请输入电话" />
        </el-form-item>
        <el-form-item label="日期" prop="date">
          <el-date-picker
            v-model="queryForm.date"
            type="date"
            placeholder="请选择日期"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleQuery">検索</el-button>
          <el-button @click="handleReset">重置</el-button>
          <el-button type="success" @click="handleExport">导出数据</el-button>
        </el-form-item>
      </el-form>
  
      <!-- 查询结果表格 -->
      <el-table :data="tableData" border style="width: 100%">
        <el-table-column prop="id" label="ID" width="80" />
        <el-table-column prop="name" label="姓名" width="120" />
        <el-table-column prop="phone" label="电话" width="150" />
        <el-table-column prop="address" label="地址" />
        <el-table-column prop="birthday" label="生日" width="120" />
        <el-table-column prop="details" label="详情" />
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
  
  <script>
  import { ref, onMounted } from 'vue';
  import { ElMessage, ElMessageBox } from 'element-plus';
//   import { exportToExcel } from '@/utils/export'; // 假设有一个导出工具函数
  
  export default {
    setup() {
      const queryForm = ref({
        name: '',
        phone: '',
        date: '',
      });
  
      const tableData = ref([]); // 表格数据
      const total = ref(0); // 总条数
      const pageSize = ref(30); // 每页显示条数
      const currentPage = ref(1); // 当前页码
  
      // 模拟数据
      const mockData = (page, pageSize, query) => {
        const data = [];
        const start = (page - 1) * pageSize;
        for (let i = 1; i <= pageSize; i++) {
          data.push({
            id: start + i,
            name: `用户${start + i}`,
            phone: `1380013800${i}`,
            address: `地址${start + i}`,
            birthday: `1990-01-${i < 10 ? '0' + i : i}`,
            details: `详情${start + i}`,
          });
        }
        return data;
      };
  
      // 查询数据
      const fetchData = () => {
        const { name, phone, date } = queryForm.value;
        // 模拟查询条件过滤
        const filteredData = mockData(currentPage.value, pageSize.value, {
          name,
          phone,
          date,
        });
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
        queryForm.value = { name: '', phone: '', date: '' };
        fetchData();
      };
  
      // 处理分页大小变化
      const handleSizeChange = (size) => {
        pageSize.value = size;
        fetchData();
      };
  
      // 处理页码变化
      const handlePageChange = (page) => {
        currentPage.value = page;
        fetchData();
      };
  
      // 处理导出数据
      const handleExport = () => {
        ElMessageBox.confirm('确认导出当前查询结果吗？', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning',
        })
          .then(() => {
            const exportData = tableData.value.map((item) => ({
              ID: item.id,
              姓名: item.name,
              电话: item.phone,
              地址: item.address,
              生日: item.birthday,
              详情: item.details,
            }));
            // exportToExcel(exportData, '查询结果');
            ElMessage.success('导出成功');
          })
          .catch(() => {
            ElMessage.info('取消导出');
          });
      };
  
      return {
        queryForm,
        tableData,
        total,
        pageSize,
        currentPage,
        handleQuery,
        handleReset,
        handleSizeChange,
        handlePageChange,
        handleExport,
      };
    },
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
  </style>