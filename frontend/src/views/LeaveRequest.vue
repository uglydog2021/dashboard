<template>
  <div class="query-page">
    <el-form
      :model="queryForm"
      ref="queryFormRef"
      label-width="120px"
      :rules="rules"
      label-position="top"
    >
      <!-- 第一行：用户信息 -->
      <el-row :gutter="20">
        <el-col :xs="24" :sm="12" :md="12" :lg="12">
          <el-form-item label="TaskUser" prop="TaskUser">
            <el-input
              v-model="queryForm.TaskUser"
              placeholder="请输入用户名称"
              clearable
            />
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="12" :md="12" :lg="12">
          <el-form-item label="Department" prop="Department">
            <el-input
              v-model="queryForm.Department"
              placeholder="请输入部门名称"
              clearable
            />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 第二行：日期范围 -->
      <el-row :gutter="20">
        <el-col :xs="24" :sm="12" :md="12" :lg="12">
          <el-form-item label="开始日期" prop="StartData">
            <el-date-picker
              v-model="queryForm.StartData"
              type="date"
              placeholder="选择开始日期"
              style="width: 100%"
              value-format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="12" :md="12" :lg="12">
          <el-form-item label="结束日期" prop="EndData">
            <el-date-picker
              v-model="queryForm.EndData"
              type="date"
              placeholder="选择结束日期"
              style="width: 100%"
              value-format="YYYY-MM-DD"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 第三行：时间与类型 -->
      <el-row :gutter="20">
        <el-col :xs="24" :sm="12" :md="12" :lg="12">
          <el-form-item label="总时长（小时）" prop="TotalTime">
            <el-input
              v-model="queryForm.TotalTime"
              :min="1"
              :max="240"
              controls-position="right"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="12" :md="12" :lg="12">
          <el-form-item label="休假类型" prop="Type">
            <el-select
              v-model="queryForm.Type"
              placeholder="请选择休假类型"
              filterable
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

      <!-- 第四行：详细说明 -->
      <el-row :gutter="20">
        <el-col :span="24">
          <el-form-item label="申请理由" prop="Reason">
            <el-input
              v-model="queryForm.Reason"
              placeholder="请输入详细申请理由"
              type="textarea"
              :rows="4"
              show-word-limit
              maxlength="200"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- 操作按钮 class="action-container" -->
      
    </el-form>
    <div class="action-container">
        <el-button type="primary">查询</el-button>
        <el-button @click="handleReset">重置</el-button>
        <el-button type="success">导出数据</el-button>
      </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { storeToRefs } from 'pinia';
import { useSettingStore } from '@/stores/index';
import { currentGET } from '@/api';
import { typeOption } from './ScreenDesign2/component/consts';

// 表单验证规则
const validateEndDate = (rule: any, value: string, callback: Function) => {
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
    { validator: validateEndDate, trigger: 'change' },
  ]
};

// 数据初始化
const settingStore = useSettingStore();
const { dbArray } = storeToRefs(settingStore);
settingStore.getDatebase();

const queryForm = ref({
  StartData: '',
  EndData: '',
  TaskUser: '',
  Type: '',
  TotalTime: '',
  Department: '',
  Reason: '',
});

const queryFormRef = ref(); // 表单引用

// 处理重置
const handleReset = () => {
  queryFormRef.value.resetFields(); // 重置表单数据和验证状态
  // fetchData();
};

</script>

<style scoped>
.query-page {
  /* max-width: 1200px; */
  margin: 20px auto;
  padding: 20px;
  background: #fff;
  /* border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1); */
}

.action-container {
  display: flex;
  justify-content: flex-end;
  margin-top: 24px;
}

/* 响应式适配 */
@media (max-width: 768px) {
  .el-form-item__label {
    font-size: 14px;
  }
  
  .action-container {
    justify-content: center;
  }
}
</style>