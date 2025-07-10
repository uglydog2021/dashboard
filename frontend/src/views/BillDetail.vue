<template>
  <div class="timeline-page">
    <!-- 标题 -->
    <h2>单据处理历史 - {{ FileName }}</h2>

    <!-- 空数据提示 -->
    <el-empty v-if="!loading && timeLineData.length === 0" description="暂无数据" />

    <!-- 时间轴 -->
    <el-timeline v-loading="loading">
      <!-- 遍历历史记录 -->
      <el-timeline-item
        v-for="(item, index) in timeLineData"
        :key="index"
        :timestamp="dateTimeFormat(item.createDate)"
        type="info"
        placement="top"
      >
        <!-- 卡片内容 -->
        <el-card>
          <h4>{{ item.fileName }}</h4>
          <p>{{ item.taskUser }}</p>
          <p>{{ formatStatus(item.status) }}</p>
          <p>{{ item.HistoryFilePath }}</p>
          <p>{{ dateTimeFormat(item.ActionStartDate) }}</p>
          <p>{{ dateTimeFormat(item.ActionEndDate) }}</p>
          <p v-if="item.taskUser">操作人: {{ item.taskUser }}</p>
        </el-card>
      </el-timeline-item>
    </el-timeline>
  </div>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router';
import { ref, onMounted } from 'vue';
import { currentGET } from '@/api';
import { statusOption, formatStatus } from './ScreenDesign2/component/consts';
import { dateTimeFormat } from '@/utils'

// 定义时间轴数据的类型
interface TimelineItem {
  Id: string; // 操作人（可选）
  createDate: string; // 创建日期
  fileName: string; // 标题
  status: string; // 描述
  taskUser: string; // 描述
  ActionStartDate: string; // 描述
  ActionEndDate: string; // 描述
  HistoryFilePath: string; // 描述
  WorkTimeH: string; // 描述
  WorkTimeM: string; // 描述
  WorkTimeS: string; // 描述
}

const route = useRoute();
const loading = ref(false); // 加载状态
const timeLineData = ref<TimelineItem[]>([]); // 时间轴数据，指定类型为 TimelineItem[]

// 获取动态路由参数和查询参数
const FileName = route.params.fileName as string;
const ConnectionName = route.query.ConnectionName as string;

// 初始化加载数据
onMounted(() => {
  fetchData();
});

// 获取数据
const fetchData = async () => {
  loading.value = true;
  try {
    const actionData = await currentGET('GetActionHistory', {
      FileName,
      // ConnectionName,
    });
    // 假设 actionData 是一个数组，且符合 TimelineItem 结构
    timeLineData.value = actionData.result as TimelineItem[];
  } catch (error) {
    console.error('数据加载失败', error);
    // 可以在这里添加用户提示，例如使用 ElMessage 显示错误信息
  } finally {
    loading.value = false;
  }
};

</script>

<style scoped>
.timeline-page {
  padding: 20px;
  max-width: 800px;
  margin: 0 auto;
}

h2 {
  text-align: center;
  margin-bottom: 20px;
  font-size: 24px;
  color: #303133;
}

.el-timeline {
  margin-top: 20px;
}

.el-card {
  margin: 10px 0;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.el-card h4 {
  margin: 0;
  font-size: 16px;
  font-weight: bold;
  color: #303133;
}

.el-card p {
  margin: 5px 0;
  color: #606266;
}

/* 空数据提示样式 */
.el-empty {
  margin-top: 40px;
}
</style>