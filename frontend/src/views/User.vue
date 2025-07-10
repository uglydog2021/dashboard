<template>
  <div class="query-page">
    <!-- 查询条件 -->
    <el-form
      :model="queryForm"
      ref="queryFormRef"
      label-width="120px"
    >
      <el-row :gutter="20">
        <!-- UserName -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="UserName" prop="UserName">
            <el-input
              v-model="queryForm.UserName"
              placeholder="UserName"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>

        <!-- StartData -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="Phone" prop="Phone">
            <el-input
              v-model="queryForm.Phone"
              placeholder="Phone"
              style="width: 100%"
            />
            <!-- <el-date-picker
              v-model="queryForm.StartDate"
              type="date"
              placeholder="请选择日期"
              style="width: 100%"
            /> -->
          </el-form-item>
        </el-col>

        <!-- EndData 
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="EndDate" prop="EndDate">
            <el-date-picker
              v-model="queryForm.EndDate"
              type="date"
              placeholder="请选择日期"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>-->

        <!-- Publisher 
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="Publisher" prop="Publisher">
            <el-input
              v-model="queryForm.Publisher"
              placeholder="Publisher"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>-->
      </el-row>
      <div class="actionContainer">
          <el-button type="primary" @click="handleQuery">検索</el-button>
          <el-button type="warning" @click="handleEdit">編集</el-button>
          <el-button type="danger" @click="handleDelete">削除</el-button>
          <el-button type="success" @click="addNewData">新規追加</el-button>
      </div>
    </el-form>

    <!-- 查询结果表格 -->
    <el-table highlight-current-row
    style="width: 100%"
    @current-change="handleCurrentChange" :data="tableData" border v-loading="loading">
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
    <el-dialog v-model="showDialog">
      <template #header>
        <h2 class="custom-title">{{flag === 0 ? '新規追加' : '編集'}}</h2>
      </template>
        <el-form 
          :model="editForm"
          ref="editFormRef"
          label-width="120px"
          class="form-container">
          <!-- 第一行：输入框组 -->
          <el-row :gutter="20">
            <el-col :span="20">
              <el-form-item label="UserName" prop="UserName">
                <el-input v-model="editForm.UserName" placeholder="请输入UserName" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="20">
              <el-form-item label="Phone" prop="Phone">
                <!-- <el-date-picker
                  v-model="queryForm.StartDate"
                  type="date"
                  placeholder="请选择日期"
                  style="width: 100%"
                /> -->
                <el-input v-model="editForm.Phone" placeholder="请输入Phone" />
              </el-form-item>
            </el-col>
          </el-row>
          <!-- <el-row :gutter="20">
            <el-col :span="20">
              <el-form-item label="EndDate" prop="EndDate">
                <el-date-picker
                  v-model="queryForm.EndDate"
                  type="date"
                  placeholder="请选择日期"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
          </el-row> -->
          <el-row :gutter="20">
            <el-col :span="20">
              <el-form-item label="Email" prop="Email">
                <el-input
                  v-model="editForm.Email"
                  placeholder="Email"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>
        <template #footer>
          <div class="dialog-footer">
            <el-button @click="showDialog = false">キャンセル</el-button>
            <el-button type="primary" @click="SaveUserInfo">
              確定
            </el-button>
          </div>
        </template>
  </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useSettingStore } from '../stores/index';
import { currentGET, currentPOST, currentPUT } from '../api';
import { ElMessage, ElMessageBox } from 'element-plus'

export interface DataBaseDTO {
  id: number
  UserName: string
  Phone: string
  Email: string
}

// 表单验证规则
// const validateEndDate = (rule: any, value: string, callback: Function) => {
//   if (queryForm.value.StartDate && !value) {
//     callback(new Error('请选择结束日期'));
//   } else if (queryForm.value.StartDate && value <= queryForm.value.StartDate) {
//     callback(new Error('结束日期必须大于开始日期'));
//   } else {
//     callback();
//   }
// };

// const rules = {
//   EndDate: [
//     { validator: validateEndDate, trigger: 'change' },
//   ]
// };


const currentRow = ref<DataBaseDTO>();
const showDialog = ref(false);
const handleCurrentChange = (val: DataBaseDTO | undefined) => {
  currentRow.value = val!
  console.log("handleCurrentChange", val)
}
// 表格列配置
const tableColumns = [
  { prop: 'id', label: '数据库ID' ,width: '160'},
  { prop: 'UserName', label: '数据库名字' ,width: '160'},
  { prop: 'Phone', label: '账户' },
  { prop: 'Email', label: '邮箱' },
  // { prop: 'EndDate', label: '结束时间' },
  // { prop: 'Publisher', label: '发布者'},
];

// 根据状态值获取对应的文本描述
// const formatStatus = (status: string) => {
//   const statusItem = statusOption.find((item) => item.value === status);
//   return statusItem ? statusItem.label : '未知状态';
// };

// 表单验证规则
// const validateEndDate = (rule: any, value: string, callback: Function) => {
//   if (queryForm.value.StartData && !value) {
//     callback(new Error('请选择结束日期'));
//   } else if (queryForm.value.StartData && value <= queryForm.value.StartData) {
//     callback(new Error('结束日期必须大于开始日期'));
//   } else {
//     callback();
//   }
// };

// const rules = {
//   ConnectionName: [{ required: true, message: '请选择连接名称', trigger: 'change' }],
//   EndData: [
//     { validator: validateEndDate, trigger: 'change' },
//   ]
// };

// 数据初始化
const settingStore = useSettingStore();
// const { dbArray } = storeToRefs(settingStore);
settingStore.getDatebase();
// const dbs = computed(() => dbArray.value);

const queryForm = ref({
  UserName: '',
  Phone: '',
});

const editForm = ref({
  UserName: '',
  Phone: '',
  Email: ''
});

const tableData = ref([]); // 表格数据
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
    const filteredData = await currentGET('GetUserInfo', {
      Index: currentPage.value,
      PageSize: pageSize.value,
      ...queryForm.value,
    });
    tableData.value = filteredData;
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

const SaveUserInfo = async () => {
  showDialog.value = false
  if(editForm.value){
    const filteredData = await currentPUT('SaveUserInfo', { ...editForm.value });
    fetchData();
  }
}
const addNewData = () => {
  flag.value = 0;
  editForm.value = {
    UserName: '',
    Phone: '',
    Email: ''}
  showDialog.value = true
}
// 处理查询
const handleQuery = () => {
  queryFormRef.value.validate((valid: boolean) => {
    if (valid) {
      currentPage.value = 1; // 重置到第一页
      fetchData();
    } else {
      console.log('表单验证失败');
    }
  });
  }
const handleEdit = () => {
  if(currentRow.value && currentRow.value.id != null){
    flag.value = 1;
    editForm.value = currentRow.value
    showDialog.value = true
  }else{
    ElMessage({
      message: '尚未选中编辑项！',
      type: 'warning',
    })
  }
}

const handleDelete = () => {
  if(currentRow.value){
    ElMessageBox.confirm('Are you sure to close this dialog?')
    .then( async () => {
      if(currentRow.value && currentRow.value.id != null){
        const filteredData = await currentDelete('DeletePermission', { id: currentRow.value.id });
        fetchData();
      }
    })
    .catch(() => {
      console.error("delete permission data error")
    })
  }else{
    ElMessage({
      message: '尚未选中编辑项！',
      type: 'warning',
    })
  }
}
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

function currentDelete(arg0: string, arg1: { id: number; }) {
  throw new Error('Function not implemented.');
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
  gap: 12px;  /* 现代浏览器间距方案 */
  margin-top: 16px;  /* 添加顶部间距 */
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