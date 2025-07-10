<template>
  <div id="content">
    <div class="topContentBox">
      <div class="contentItemBox">
        <p>baseline件数</p>
        <p>
          <span>{{ countData.baseline ?? 0 }}件</span>
        </p>
      </div>
      <div class="contentItemBox">
        <p>本月共计</p>
        <p>
          <span>{{ countData.totalFileCount ?? 0 }}件</span>
        </p>
      </div>
      <div class="contentItemBox">
        <p>本月达成</p>
        <p>
          <span>{{ countData.baselineRate ?? 0 }}%</span>
        </p>
      </div>
    </div>
    <!-- 中间内容盒 -->
    <div class="centerContentBox">
      <div class="centerTop">
        <h3>当日受领件数</h3>
        <p>
          <span>{{ countData.totalFileCount2 ?? 0 }}</span>
        </p>
      </div>
      <!-- 中间左上块的内容 -->
      <div class="showProjectNumBox waitProjectNumBox">
        <v-chart
          id="waitEchart"
          class="ringPicBoxLeft"
          :option="waitLastEchartOption"
          v-if="JSON.stringify(waitLastEchartOption) != '{}'"
        />
        <div class="descBoxLeft">
          <p class="proNum">{{ countData.waitMaker }}</p>
          <h3 class="proNumName">等待Marker</h3>
        </div>
      </div>
      <!-- 中间左中块的内容 -->
      <div class="showProjectNumBox lastBeforeProjectNumBox">
        <v-chart
          id="lastEchart"
          class="ringPicBoxLeft"
          :option="lastEchartOption"
          v-if="JSON.stringify(lastEchartOption) != '{}'"
        />
        <div class="descBoxLeft">
          <p class="proNum">{{ countData.makerUrgent }}</p>
          <h3 class="proNumName">等待Marker至急</h3>
        </div>
      </div>

      <!-- 中间左下块的内容 -->
      <div class="showProjectNumBox lastProjectNumBox">
        <v-chart
          id="lastBeforeEchart"
          class="ringPicBoxLeft"
          :option="lastBeforeEchartOption"
          v-if="JSON.stringify(lastBeforeEchartOption) != '{}'"
        />
        <div class="descBoxLeft">
          <p class="proNum">{{ countData.makerComplete ?? 0 }}</p>
          <h3 class="proNumName">Marker完成</h3>
        </div>
      </div>

      <!-- 中间右上块的内容 -->
      <div class="showProjectNumBox waitbeforeProjectNumBox">
        <v-chart
          id="beforeEchart"
          class="ringPicBoxRight"
          :option="waitBeforeEchartOption"
          v-if="JSON.stringify(waitBeforeEchartOption) != '{}'"
        />
        <div class="descBoxRight">
          <p class="proNum">{{ countData.waitChecker }}</p>
          <h3 class="proNumName">等待Checker</h3>
        </div>
      </div>
      <!-- 中间右中块的内容 -->
      <div class="showProjectNumBox beforeProjectNumBox">
        <v-chart
          id="beforeEchart"
          class="ringPicBoxRight"
          :option="beforeEchartOption"
          v-if="JSON.stringify(beforeEchartOption) != '{}'"
        />
        <div class="descBoxRight">
          <p class="proNum">{{ countData.checkerUrgent ?? 0 }}</p>
          <h3 class="proNumName">等待Checker至急</h3>
        </div>
      </div>

      <!-- 中间右下块的内容 -->
      <div class="showProjectNumBox nowProjectNumBox">
        <v-chart
          id="nowEchart"
          class="ringPicBoxRight"
          :option="nowEchartOption"
          v-if="JSON.stringify(nowEchartOption) != '{}'"
        />
        <div class="descBoxRight">
          <p class="proNum">{{ countData.checkerComplete ?? 0 }}</p>
          <h3 class="proNumName">Checker完成</h3>
        </div>
      </div>
    </div>

    <!-- 底部选项卡 -->
    <div class="bottomContentBox">
      <div ref="popoverContainer" class="side" @click="showPopper = true">
        <p :class="'unactiveLeft'">
          {{ currentGroup.label }}
        </p>
        <div
          ref="popover"
          v-if="showPopper"
          tabindex="-1"
          class="pop-container"
        >
          <el-tree
            ref="teamTree"
            style="width: 100%"
            :data="groupData"
            :highlight-current="true"
            :default-expand-all="true"
            node-key="id"
            :current-node-key="currentGroup.id"
            :indent="10"
            :props="defaultProps"
            @node-click="handleNodeClick"
          />
        </div>
      </div>
      <div class="center" @click="showDialog">
        <p :class="'unactiveCenter'">Pending件数 {{ gridData.length }}</p>
      </div>
      <div class="center">
        <p :class="'unactiveCenter'" style="color: #293855"></p>
      </div>
      <div class="side">
        <p :class="'unactiveRight'" style="color: #293855"></p>
      </div>
    </div>
  </div>
  <el-dialog
    :header-class="'custom-header'"
    width="100%"
    v-model="isDialogVisible"
    style="background-color: #27314d94 !important"
  >
    <template #header>
      <h2 class="custom-title">Pending</h2>
    </template>
    <el-table :data="gridData" class="custom-bg-table">
      <el-table-column property="fileName" label="fileName" width="510px" />
      <el-table-column property="taskUser" label="taskUser" />
    </el-table>
  </el-dialog>
</template>

<script setup lang="ts">
import { nextTick, ref, watch, type Ref } from "vue";
import { StatusCodeTotalCountModel } from "@/views/ScreenDesign2/ScreenDesign2.vue";
import useWatchDateRange from "@/hooks/use-watch";
import { useSettingStore } from "@/stores";
import { storeToRefs } from "pinia";

const settingStore = useSettingStore();
const { dateRange } = storeToRefs(settingStore);
const countData = ref<StatusCodeTotalCountModel>({});
const gridData = ref<any[]>([]);
const waitLastEchartOption = ref({});
const lastEchartOption = ref({});
const lastBeforeEchartOption = ref({});
const waitBeforeEchartOption = ref({});
const beforeEchartOption = ref({});
const nowEchartOption = ref({});
const showPopper = ref(false);
const isDialogVisible = ref(false);
const popover = ref<HTMLDivElement | null>(null);
const popoverContainer = ref<HTMLDivElement | null>(null);
const teamTree = ref<HTMLDivElement | null>(null);
const emit = defineEmits(["open-dialog"]);
const groupLink: Record<string, string> = {
  "1": "DS",
  "3": "DS",
  "17": "IS",
  "20": "IS",
  "21": "IS",
};
const groupData = ref([
  {
    id: "DS",
    label: "直販",
    disabled: true,
    children: [
      {
        id: "1",
        label: "東京",
      },
      {
        id: "3",
        label: "大阪",
      },
    ],
  },
  {
    id: "IS",
    label: "間販",
    disabled: true,
    children: [
      {
        id: "17",
        label: "入力A",
      },
      {
        id: "20",
        label: "入力B",
      },
      {
        id: "21",
        label: "新SC",
      },
    ],
  },
]);
const defaultProps = ref({
  children: "children",
  label: "label",
  disabled: "disabled",
});
const currentGroup = ref({ id: "1", label: "东京" });
dateRange.value!.group = currentGroup.value.id;
dateRange.value!.team = groupLink[currentGroup.value.id];

const showDialog = () => {
  isDialogVisible.value = true;
};

const handleClickOutside = (e: MouseEvent) => {
  if (
    popoverContainer.value &&
    !popoverContainer.value.contains(e.target as Node)
  ) {
    showPopper.value = false;
  }
};
watch(showPopper, async (newVal: any) => {
  if (newVal) {
    await nextTick();
    document.addEventListener("click", handleClickOutside);
  } else {
    document.removeEventListener("click", handleClickOutside);
  }
});
const handleNodeClick = (data: any, node: any, treeNode: any, e: Event) => {
  if (!data.disabled) {
    currentGroup.value.id = data.id;
    currentGroup.value.label = data.label;
    showPopper.value = false;
    dateRange.value!.group = data.id;
    dateRange.value!.team = groupLink[data.id];
    console.log("dateRange", dateRange);
  }
  e.preventDefault();
  // myPopover.value?.hide();
};
const setEcharts = (optionRef: Ref<any>, rateNum: number) => {
  optionRef.value = {
    series: [
      {
        type: "gauge",
        // 开始的旋转角度
        startAngle: 90,
        endAngle: -270,
        // 线性渐变，前四个参数分别是 x0, y0, x2, y2, 范围从 0 - 1，相当于在图形包围盒中的百分比，如果 globalCoord 为 `true`，则该四个值是绝对的像素位置
        color: {
          type: "linear",
          x: 0,
          y: 0,
          x2: 0,
          y2: 1,
          colorStops: [
            {
              offset: 0,
              color: "#BABA24", // 0% 处的颜色
            },
            {
              offset: 1,
              color: "#11FCFA", // 100% 处的颜色
            },
          ],
          global: false, // 缺省为 false
        },
        pointer: {
          show: false,
        },
        progress: {
          show: true,
          overlap: false,
          // 两头是否是圆弧
          roundCap: false,
          clip: false,
          itemStyle: {
            borderWidth: 0,
          },
        },
        axisLine: {
          lineStyle: {
            // 环的宽度
            width: 6,
            // 没有覆盖的背景颜色
            color: [[1, "transparent"]],
          },
        },
        splitLine: {
          show: false,
        },
        axisTick: {
          show: false,
        },
        axisLabel: {
          show: false,
        },
        data: [
          {
            value: rateNum,
            name: "Perfect",
            detail: {
              valueAnimation: true,
              offsetCenter: ["0%", "10%"],
            },
          },
        ],
        title: {
          show: false,
          fontSize: 14,
        },
        detail: {
          width: 50,
          height: 14,
          fontSize: 14,
          fontWeight: 900,
          color: "#00FBFC",
          borderColor: "inherit",
          borderRadius: 20,
          borderWidth: 0,
          formatter: `${rateNum}%`,
        },
      },
    ],
  };
};

// 切换选项卡时数据重新赋值与重新绘制echarts图
const changeChooseCards = () => {
  // 等待Marker
  if (countData.value.totalFileCount2) {
    if (countData.value.waitMaker) {
      setEcharts(
        waitLastEchartOption,
        Math.round(
          (countData.value.waitMaker / countData.value.totalFileCount2) * 100
        )
      );
    }
    if (countData.value.makerUrgent) {
      setEcharts(
        lastEchartOption,
        Math.round(
          (countData.value.makerUrgent / countData.value.totalFileCount2) * 100
        )
      );
    }
    if (countData.value.makerComplete) {
      setEcharts(
        lastBeforeEchartOption,
        Math.round(
          (countData.value.makerComplete / countData.value.totalFileCount2) * 100
        )
      );
    }
    if (countData.value.waitChecker) {
      setEcharts(
        waitBeforeEchartOption,
        Math.round(
          (countData.value.waitChecker / countData.value.totalFileCount2) * 100
        )
      );
    }
    if (countData.value.checkerUrgent) {
      setEcharts(
        beforeEchartOption,
        Math.round(
          (countData.value.checkerUrgent / countData.value.totalFileCount2) *
            100
        )
      );
    }
    if (countData.value.checkerComplete) {
      setEcharts(
        nowEchartOption,
        Math.round(
          (countData.value.checkerComplete / countData.value.totalFileCount2) *
            100
        )
      );
    }
  }
};
const setCountData = (r: any[]) => {
  if (r && r.length > 0) {
    const data1 = r[0].result;
    const data2 = r[1].result;
    const data3 = r[2].result;
    countData.value = {
      ...data1,
      totalFileCount2: data2.totalFileCount,
      ...data3,
    };
    gridData.value = r[3].result;
    changeChooseCards();
  }
};
useWatchDateRange(
  [
    "getMonthFileCount",
    "getDailyTotalFileCount",
    "getDailyCountByStatus",
    "getDailyPendingList",
  ],
  setCountData
);
defineExpose({ changeChooseCards });
</script>

<style scoped>
#content {
  width: 100%;
  height: 100%;
  background-image: url(../../../assets/images/中间1.png);
  background-size: 100% auto;
  background-repeat: no-repeat;
  background-position: center; /* 可选：水平居中 */
  /* background: url(../../../assets/images/中间1.png) no-repeat center center; */
}

.topContentBox {
  height: 99px;
  margin: 40px 10px 0;
  display: flex;
}

.contentItemBox {
  flex-grow: 1;
  margin: 0 10px;
  box-shadow: 0 0 20px #00e0e9 inset;
  color: #fff;
  /* border: 2px solid #00E0E9; */
  background: linear-gradient(to left, #00e0e9, #00e0e9) left top no-repeat,
    linear-gradient(to left, #00e0e9, #00e0e9) left top no-repeat,
    linear-gradient(to left, #00e0e9, #00e0e9) right top no-repeat,
    linear-gradient(to left, #00e0e9, #00e0e9) right top no-repeat,
    linear-gradient(to left, #00e0e9, #00e0e9) left bottom no-repeat,
    linear-gradient(to left, #00e0e9, #00e0e9) left bottom no-repeat,
    linear-gradient(to left, #00e0e9, #00e0e9) right bottom no-repeat,
    linear-gradient(to left, #00e0e9, #00e0e9) right bottom no-repeat;
  background-size: 2px 20px, 20px 2px;
}

.contentItemBox p {
  text-align: center;
}

.contentItemBox p:nth-child(1) {
  margin-top: 10px;
  font-size: 16px;
  line-height: 30px;
}

.contentItemBox p:nth-child(2) {
  margin-bottom: 10px;
}

.contentItemBox p:nth-child(2) span {
  font-size: 35px;
  font-weight: bold;
  color: #006ede;
}

.contentItemBox:nth-child(2) p:nth-child(2) span {
  color: #00d2ee;
}

.contentItemBox:nth-child(3) p:nth-child(2) span {
  color: #fcdc7a;
}

/* 中间样式 */
.centerContentBox {
  position: relative;
  width: 100%;
  height: 680px;
}

.centerContentBox .centerTop {
  position: absolute;
  top: 50px;
  left: 50%;
  transform: translate(-50%, 0);
  width: 280px;
  height: 100px;
  text-align: center;
}

.centerContentBox .centerTop h3 {
  margin-top: 16px;
  color: #fff;
  font-size: 20px;
  line-height: 30px;
}

.centerContentBox .centerTop p {
  color: #00b3fd;
  font-size: 20px;
  font-weight: bold;
}

.centerContentBox .centerTop p span {
  font-size: 40px;
  text-shadow: 0 0 26px #ffffff;
}

.showProjectNumBox {
  position: absolute;
  width: 220px;
  height: 74px;
}

.showProjectNumBox .ringPicBoxLeft {
  position: absolute;
  left: 7px;
  top: 1px;
  width: 70px;
  height: 70px;
}

.showProjectNumBox .ringPicBoxRight {
  position: absolute;
  right: 7px;
  top: 1px;
  width: 70px;
  height: 70px;
}

.showProjectNumBox .descBoxLeft {
  position: absolute;
  left: 90px;
}

.showProjectNumBox .descBoxRight {
  position: absolute;
  right: 90px;
  text-align: right;
}

.showProjectNumBox .proNum {
  font-size: 30px;
  font-weight: bold;
  letter-spacing: -2px;
  color: #007ffc;
}

.showProjectNumBox .proNumName {
  font-size: 14px;
  font-weight: normal;
  color: #fff;
}

.waitProjectNumBox {
  top: 112px;
  left: 29px;
}

.lastBeforeProjectNumBox {
  top: 216px;
  left: 49px;
}

.lastProjectNumBox {
  top: 320px;
  left: 75px;
}

.waitbeforeProjectNumBox {
  top: 114px;
  right: 30px;
}

.beforeProjectNumBox {
  top: 215px;
  right: 43px;
}

.nowProjectNumBox {
  top: 320px;
  right: 69px;
}

/* 底部样式 */
.bottomContentBox {
  display: flex;
  width: 771px;
  height: 71px;
  background: url(../../../assets/images/背景线框.png) center center no-repeat;
  margin: 0 auto;
  margin-top: 36px;
}

.bottomContentBox .center {
  flex-grow: 1;
  width: 110px;
}

.bottomContentBox .side {
  flex-grow: 1.2;
  width: 110px;
}

.bottomContentBox p {
  width: 100%;
  height: 100%;
  cursor: pointer;
  text-align: center;
  color: #00e3fe;
  line-height: 70px;
}

.bottomContentBox .unactiveLeft {
  background: url(../../../assets/images/左蓝.png) no-repeat center center;
  background-size: 85% 70%;
}

.bottomContentBox .activeLeft {
  background: url(../../../assets/images/左黄.png) no-repeat center center;
  background-size: 85% 70%;
}

.bottomContentBox .unactiveCenter {
  background: url(../../../assets/images/中蓝.png) no-repeat center center;
}

.bottomContentBox .activeCenter {
  background: url(../../../assets/images/中黄.png) no-repeat center center;
}

.bottomContentBox .unactiveRight {
  background: url(../../../assets/images/右蓝.png) no-repeat center center;
  background-size: 85% 70%;
}

.bottomContentBox .activeRight {
  background: url(../../../assets/images/右黄.png) no-repeat center center;
  background-size: 85% 70%;
}

.pop-container {
  width: 125px;
  position: absolute;
  left: 45px;
  bottom: 75px;
  background: #37396373;
  border: 1px solid #002b8fb5;
  padding: 10px;
}

/* .trigger:focus ~ .target-div {
  display: none;
} */

.el-tree {
  background-image: url(/src/assets/images/bg.jpg);
  background-size: 1000% 300%;
  background-repeat: no-repeat;
  background-position: center;
  color: white;
  /* padding: 5px; */
  --el-tree-node-content-height: 28px;
  --el-tree-node-hover-bg-color: #195e93;
}

:deep(
    .el-tree--highlight-current
      .el-tree-node.is-current
      > .el-tree-node__content
  ) {
  background-color: var(--el-color-primary-dark-2) !important;
}
</style>
