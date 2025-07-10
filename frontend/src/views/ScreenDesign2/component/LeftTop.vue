<template>
  <div>
    <div
      id="item05cd9ca0-3dec-11ec-a2e6-953464dcce14"
      :style="{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        fontSize: '18px',
        lineHeight: '1.5',
        color: '#ffffff',
        textAlign: 'center',
        textShadow: false ? '0px 0px 14px rgba(15, 146, 255,1)' : '',
      }"
      class="centertitle"
    >
      <p>出勤比例</p>
    </div>
    <div ref="selectContainer" class="selectContainer">
      总员工人数{{ TotalNumberEmployees }}
    </div>
    <div id="item8d374050-3e10-11ec-b331-d9ec61910d4a">
      <v-chart
        class="chart"
        :option="option"
        v-if="JSON.stringify(option) != '{}'"
      />
    </div>
    <div id="itemda42a880-eaf0-4a65-b71f-a313e2bd4daf">
      <img
        src="../../../assets/images/线框.png"
        alt=""
        :style="{ width: '100%', height: '100%' }"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { EChartsOption } from "echarts/types/dist/shared";
import useWatchDateRange from "@/hooks/use-watch";

type AttendanceRecord = {
  OrganizationID: number;
  AttendanceCount: number;
};
type ResultData = {
  OrganizationID: number;
  TotalNumberEmployees: number;
  NumberAttendance: number;
  NumberPeopleVacation: number;
  NumberAnnualRestDays: number;
  NumberSickLeavePatients: number;
  OtherNumberVacationers: number;
};
const AttendanceNameMap: Record<string, string> = {
  TotalNumberEmployees: "总员工人数",
  NumberAttendance: "出勤",
  NumberPeopleVacation: "串休",
  NumberAnnualRestDays: "年休",
  NumberSickLeavePatients: "病休",
  OtherNumberVacationers: "其他休假",
};
const option = ref<EChartsOption>({});
const TotalNumberEmployees = ref(0);
const setEcharts = (datas: any) => {
  if (!datas) {
    return;
  }
  const res: ResultData[] = datas.result;
  const showPieData: any[] = [];
  if (res && res.length > 0) {
    const resultData = res[0];
    TotalNumberEmployees.value = resultData.TotalNumberEmployees;
    for (const k in resultData) {
      const t = k as keyof ResultData;
      if (t === "TotalNumberEmployees" || t === "OrganizationID") {
        continue;
      }
      showPieData.push({
        name: AttendanceNameMap[k],
        value: (
          ((resultData[t] ?? 0) / resultData.TotalNumberEmployees) *
          100
        ).toFixed(2),
        projectNums: resultData[t],
      });
    }
  }
  option.value = {
    series: [
      {
        name: "",
        type: "pie",
        radius: [50, 80],
        top: 0,
        itemStyle: {},
        label: {
          alignTo: "edge",
          formatter: "{b} : {c} %",
          minMargin: 5,
          edgeDistance: 0,
          // lineHeight: 0,
          fontSize: 16,
          color: "#fff",
          rich: {},
        },
        labelLine: {
          length: 20,
          length2: -60,
          maxSurfaceAngle: 100,
        },
        data: showPieData,
      },
    ],
  };
};
useWatchDateRange("GetSLA", setEcharts);
</script>

<style scoped>
/* 左上背景图位置及大小 */
#itemda42a880-eaf0-4a65-b71f-a313e2bd4daf {
  left: 20px;
  width: 530px;
  height: 310px;
}

/* 标题位置 */
#item05cd9ca0-3dec-11ec-a2e6-953464dcce14 {
  top: 14px;
}

/* echarts图的位置及大小 */
#item8d374050-3e10-11ec-b331-d9ec61910d4a {
  top: 5%;
  height: 270px;
}
.selectContainer {
  z-index: 11;
  display: block;
  position: absolute;
  right: 36px;
  top: 51px;
  /* padding-right: 10px; */
  color: white;
}
</style>
