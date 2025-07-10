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
          <el-form-item label="taskUser" prop="taskUser">
            <el-input
              v-model="queryForm.taskUser"
              placeholder="taskUser"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>

        <!-- taskUser -->
        <el-col :xs="24" :sm="12" :md="8" :lg="6">
          <el-form-item label="department" prop="department">
            <el-input
              v-model="queryForm.department"
              placeholder="department"
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
                v-for="item in typeOption"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <div class="actionContainer">
        <el-button type="primary" @click="handleQuery">查询</el-button>
        <!-- <el-button @click="handleReset">重置</el-button> -->
        <el-button type="warning" @click="handleEdit">编辑</el-button>
        <el-button type="danger" @click="handleDelete">删除</el-button>
        <el-button type="success" @click="addNewData">添加</el-button>
        <!-- <el-button @click="handleReset">重置</el-button>
        <el-button type="success">导出数据</el-button> -->
      </div>
    </el-form>

    <!-- 查询结果表格 -->
    <el-table highlight-current-row
    @current-change="handleCurrentChange" :data="tableData" border style="width: 100%" v-loading="loading">
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
    <el-dialog v-model="showDialog">
      <template #header>
        <h2 class="custom-title">{{flag === 0 ? '新建' : '编辑'}}</h2>
      </template>
        <el-form 
          :model="editForm"
          ref="editFormRef"
          label-width="120px"
          class="form-container">
          <!-- 第一行：输入框组 -->
          <el-row :gutter="20">
            <el-col :span="20">
              <el-form-item label="taskUser" prop="taskUser">
                <el-input v-model="editForm.taskUser" placeholder="请输入taskUser" />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="20">
              <el-form-item label="department" prop="department">
                <el-input
                  v-model="editForm.department"
                  placeholder="department"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="20">
              <el-form-item label="startDate" prop="startDate">
                <el-date-picker
                  v-model="editForm.startDate"
                  type="date"
                  placeholder="请选择日期"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="20">
              <el-form-item label="endDate" prop="endDate">
                <el-date-picker
                  v-model="editForm.endDate"
                  type="date"
                  placeholder="请选择日期"
                  style="width: 100%"
                />
              </el-form-item>
            </el-col>
          </el-row>
          <el-row :gutter="20">
            <el-col :span="20">
              <el-form-item label="Type" prop="type">
                <el-select
                  v-model="editForm.type"
                  placeholder="Select"
                  style="width: 100%"
                >
                  <el-option
                    v-for="item in typeOption"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                  />
                </el-select>
              </el-form-item>
            </el-col>
          </el-row>
          <!-- 第二行：权限单选组 -->
          
        </el-form>
        <template #footer>
          <div class="dialog-footer">
            <el-button @click="showDialog = false">Cancel</el-button>
            <el-button type="primary" @click="saveLeaveRecord">
              Confirm
            </el-button>
          </div>
        </template>
  </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { storeToRefs } from 'pinia';
import { useSettingStore } from '@/stores/index';
import { currentGET, currentPUT } from '@/api';
import { typeOption } from './ScreenDesign2/component/consts';
import { ElMessage, ElMessageBox } from 'element-plus';
const currentRow = ref<LeaveRecordDTO>();
const editFormRef = ref(); // 表单引用
const showDialog = ref(false);

const handleCurrentChange = (val: LeaveRecordDTO | undefined) => {
  currentRow.value = val!
}

export interface LeaveRecordDTO {
  id: number
  taskUser: string
  startDate: string
  endDate: string
  department: string
  type: string
}

const editForm = ref({
  taskUser: '',
  startDate: '',
  endDate: '',
  department: '',
  type: ''
});
// 表格列配置
const tableColumns = [
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

// 表单验证规则
const validateendDate = (rule: any, value: string, callback: Function) => {
  if (queryForm.value.StartData && !value) {
    callback(new Error('请选择结束日期'));
  } else if (queryForm.value.StartData && value <= queryForm.value.StartData) {
    callback(new Error('结束日期必须大于开始日期'));
  } else {
    callback();
  }
};

const rules = {
  // ConnectionName: [{ required: true, message: '请选择连接名称', trigger: 'change' }],
  EndData: [
    { validator: validateendDate, trigger: 'change' },
  ]
};

// 数据初始化
const settingStore = useSettingStore();
const { dbArray } = storeToRefs(settingStore);
settingStore.getDatebase();
const flag = ref(0);
const queryForm = ref({
  department: '',
  StartData: '',
  EndData: '',
  taskUser: '',
  Status: '',
});

const addNewData = () => {
  flag.value = 0
  editForm.value = {
    department: '',
    startDate: '',
    endDate: '',
    taskUser: '',
    type: '',
  }
  showDialog.value = true
}

const saveLeaveRecord = async () => {
  showDialog.value = false
  if(editForm.value){
    const filteredData = await currentPUT('saveLeaveRecord', { ...editForm.value });
    fetchData();
  }
}

const tableData = ref([]); // 表格数据
const total = ref(0); // 总条数
const pageSize = ref(30); // 每页显示条数
const currentPage = ref(1); // 当前页码
const loading = ref(false); // 加载状态
const queryFormRef = ref(); // 表单引用
const selectedConnName = ref(''); ;
// 查询数据
const fetchData = async () => {
  loading.value = true;
  try {
    const filteredData = await currentGET('GetLeaveRecord', {
      Index: currentPage.value,
      PageSize: pageSize.value,
      ...queryForm.value,
    });
    console.log(filteredData)
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
};

const handleEdit = () => {
  if(currentRow.value){
    flag.value = 1
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
        const filteredData = await currentGET('DeletePermission', { id: currentRow.value.id });
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