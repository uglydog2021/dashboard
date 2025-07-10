<template>
  <div class="query-page">
    <el-form
      :model="queryForm"
      ref="queryFormRef"
      label-width="120px"
      :rules="rules"
      class="form-container"
    >
      <el-row :gutter="20">
        <!-- 类型选择 -->
        <el-col :xs="24" :sm="12" :md="8" :lg="7" :xl="6" class="form-col">
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
      </el-row>
      <div class="actionContainer">
        <el-button type="primary" @click="handleQuery">検索</el-button>
        <el-button @click="handleReset">重置</el-button>
      </div>
    </el-form>
    <!-- 查询结果表格 -->
    <div @click="showDialog = true">
      <svg
        t="1744168470497"
        class="icon"
        viewBox="0 0 1024 1024"
        version="1.1"
        xmlns="http://www.w3.org/2000/svg"
        p-id="1997"
        width="200"
        height="200"
      >
        <path
          d="M127.584 128l-31.584 0 0 768 832 0 0-30.816-799.872-1.6-0.544-735.584zM352 160c0-17.696-14.336-32-32-32l-96 0c-17.696 0-32 14.304-32 32l0 640 160 0 0-640zM544 352c0-17.696-14.336-32-32-32l-96 0c-17.696 0-32 14.304-32 32l0 448 160 0 0-448zM736 544c0-17.696-14.336-32-32-32l-96 0c-17.696 0-32 14.304-32 32l0 256 160 0 0-256zM896 704l-96 0c-17.696 0-32 14.304-32 32l0 64 160 0 0-64c0-17.696-14.336-32-32-32z"
          fill="#3d50c1"
          p-id="1998"
        ></path>
      </svg>
    </div>
    <el-table
      :data="tableData"
      border
      style="width: 100%; margin-top: 50px"
      v-loading="loading"
    >
      <el-table-column prop="taskUser" label="taskUser" />
      <el-table-column prop="User_Name" label="userNameDisplay" />
      <el-table-column prop="TotalMarkAll" label="TotalMarkAll" />
      <!--      <el-table-column v-if="queryForm.Type === '2'" prop="TotalCheckerAll" label="TotalCheckerAll" />-->
    </el-table>
    <br />
    <el-dialog
      v-model="showDialog"
      title="图表展示"
      width="80%"
      @opened="initChart"
      @closed="destroyChart"
    >
      <div ref="chartRef" class="chart-container"></div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, computed } from "vue";
import { currentGET } from "@/api";
import * as echarts from "echarts";
import { useSettingStore } from "@/stores";
import { storeToRefs } from "pinia";

export interface TotalAllModel {
  taskUser?: string;
  User_Name: string;
  TotalMarkAll?: number;
}
type QueryModel = {
  ConnectionName?: string;
  ActionStartDate?: string;
  ActionEndDate?: string;
};
const settingStore = useSettingStore();
const { dbArray } = storeToRefs(settingStore);
settingStore.getDatebase();
const dbs = computed(() => dbArray.value);
const queryForm = ref<QueryModel>({});
const loading = ref(false); // 加载状态
const showDialog = ref(false); // 加载状态
const queryFormRef = ref(); // 表单引用
const tableData = ref([]); // 表格数据
const option = ref<echarts.EChartsOption>({});
const chartRef = ref(null);

let chartInstance: echarts.ECharts | null = null;

// 初始化图表
const initChart = () => {
  if (!chartRef.value) return;
  destroyChart(); // 先销毁旧实例
  chartInstance = echarts.init(chartRef.value);
  chartInstance.setOption(setEchart(tableData.value));
  // 首次渲染后强制刷新
  setTimeout(() => chartInstance!.resize(), 50);
};
// 销毁图表
const destroyChart = () => {
  if (chartInstance) {
    chartInstance.dispose();
    chartInstance = null;
  }
};
// 窗口resize监听
const handleResize = () => {
  chartInstance?.resize();
};
onBeforeUnmount(() => {
  window.removeEventListener("resize", handleResize);
  destroyChart();
});
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

const showEchartsData: (echarts.SeriesOption & { data: number[] })[] = [
  {
    name: "件数",
    type: "bar",
    barMaxWidth: 30, // 柱图宽度
    data: [],
    label: {
      // 显示数值的配置
      show: true, // 开启显示
      position: "top", // 数值显示在柱形上方
      formatter: "{c}", // 显示数据值，{c} 表示数据值
      color: "white",
      fontSize: "8px",
    },
  },
];

// 查询数据
const fetchData = async () => {
  loading.value = true;
  try {
    const filteredData = await currentGET("GetBackDataList", {
      ...queryForm.value,
    });
    tableData.value = filteredData.result;
  } catch (error) {
    console.error("数据加载失败", error);
  } finally {
    loading.value = false;
  }
};

const setEchart = (datas: TotalAllModel[]) => {
  const xData: any[] = [];
  showEchartsData[0].data = [];
  datas.forEach((item) => {
    xData.push(item.User_Name);
    showEchartsData[0].type = "bar";
    showEchartsData[0].data.push(item.TotalMarkAll!);
  });
  return {
    legend: {
      data: ["件数"],
      textStyle: {
        color: "#303133",
      },
    },
    grid: {
      containLabel: false, // 是否自适应x, y 轴文字的宽度
    },
    xAxis: [
      {
        type: "category",
        data: xData,
        axisPointer: {
          type: "shadow",
        },
        axisLabel: {
          interval: 0, // 全部展示
          rotate: 0, // x轴字体倾斜角度
        },
      },
    ],
    yAxis: [
      {
        type: "value",
        name: "件数",
        splitNumber: 5, // 大致分为 5 段
        // interval: 50,
        axisLabel: {
          formatter: "{value}",
        },
        splitLine: {
          show: true, // 显示左侧网格线
        },
      },
    ],
    series: showEchartsData,
  };
};

// 初始化加载数据
onMounted(() => {
  window.addEventListener("resize", handleResize);
  fetchData();
});

// 处理查询
const handleQuery = () => {
  queryFormRef.value.validate((valid: boolean) => {
    if (valid) {
      fetchData();
    } else {
      console.log("表单验证失败");
    }
  });
};
const handleReset = () => {
  queryFormRef.value.resetFields(); // 重置表单数据和验证状态
  fetchData();
};
</script>

<style scoped>
.query-form-row {
  margin-bottom: 10px;
}

.icon {
  height: 30px;
  width: 30px;
  position: absolute;
  right: 20px;
}

.chart-container {
  min-height: 400px;
}
.query-page {
  padding: 20px;
}
.actionContainer {
  display: flex;
  justify-content: flex-end;
  margin-bottom: 20px;
}
</style>
