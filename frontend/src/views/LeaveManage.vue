<template>
  <div class="bg-white rounded-xl p-6 card-shadow mb-6">
    <div class="flex items-center justify-between mb-6">
      <h3 class="font-semibold text-lg text-gray-800">导入设置</h3>
      <div class="flex items-center space-x-4">
        <button
          class="px-4 py-2 bg-primary text-white rounded-lg hover:bg-primary/90 transition-colors"
        >
          <i class="fa-solid fa-question-circle mr-1"></i> 帮助
        </button>
      </div>
    </div>

    <!-- 新增的单选项目区域 -->
    <div class="mb-6">
      <h4 class="font-medium text-gray-700 mb-3">选择导入类型</h4>
      <el-radio-group v-model="importType">
        <div class="space-y-3">
          <div class="flex items-center">
            <el-radio :value="1">SkillMatrix</el-radio>
          </div>
          <div class="flex items-center">
            <el-radio :value="2">休假管理</el-radio>
          </div>
          <div class="flex items-center">
            <el-radio :value="3">日本假期</el-radio>
          </div>
        </div>
      </el-radio-group>
    </div>

    <!-- 文件上传区域 -->
    <el-upload
      ref="upload"
      drag
      class="upload-demo"
      :limit="1"
      :on-exceed="handleExceed"
      :auto-upload="false"
      show-file-list
      v-model:file-list="fileList"
    >
      <el-icon class="el-icon--upload">
        <upload-filled />
      </el-icon>
      <div class="el-upload__text">
        Drop file here or <em>click to upload</em>
      </div>
      <div class="el-upload__text">支持的格式: XLSX, CSV, XML (最大 50MB)</div>
    </el-upload>

    <!-- 导入选项 -->
    <!-- <div class="mt-6 grid grid-cols-1 md:grid-cols-2 gap-6">
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">导入模式</label>
        <el-select
            v-model="importMod"
            placeholder="Select"
            size="large"
            style="width: 100%"
        >
          <el-option
              label="追加导入"
              :value="1"
          />
          <el-option
              label="覆盖导入"
              :value="2"
          />
          <el-option
              label="智能合并"
              :value="3"
          />
        </el-select>
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">字符编码</label>
        <el-select
            v-model="charset"
            placeholder="Select"
            size="large"
            style="width: 100%"
        >
          <el-option
              label="UTF-8"
              :value="1"
          />
          <el-option
              label="GBK"
              :value="2"
          />
          <el-option
              label="Shift-JIS"
              :value="3"
          />
          <el-option
              label="其他"
              :value="4"
          />
        </el-select>
      </div>
    </div> -->

    <!-- 导入按钮 -->
    <div class="mt-6 flex justify-end">
      <el-button :loading="loading" type="primary" @click="doUpload">
        开始导入
        <el-icon class="el-icon--right">
          <Upload />
        </el-icon>
      </el-button>
    </div>
  </div>
  <el-table
    v-if="importType === 1 && importSuccess"
    :data="tableData"
    border
    style="width: 100%"
  >
    <el-table-column prop="guid" label="guid" width="120"></el-table-column>
    <el-table-column prop="team" label="team" width="100"></el-table-column>
    <el-table-column prop="group1" label="group1" width="100"></el-table-column>
    <el-table-column prop="group2" label="group2" width="100"></el-table-column>
    <el-table-column
      prop="tokyoSkill"
      label="tokyoSkill"
      width="100"
    ></el-table-column>
    <el-table-column
      prop="osakaSkill"
      label="osakaSkill"
      width="100"
    ></el-table-column>
    <el-table-column
      prop="wave10Skill"
      label="wave10Skill"
      width="100"
    ></el-table-column>
    <el-table-column
      prop="inputASkill"
      label="inputASkill"
      width="100"
    ></el-table-column>
    <el-table-column
      prop="inputBSkill"
      label="inputBSkill"
      width="100"
    ></el-table-column>
    <el-table-column
      prop="newSCSkill"
      label="newSCSkill"
      width="100"
    ></el-table-column>
    <el-table-column
      prop="efaxSkill"
      label="efaxSkill"
      width="100"
    ></el-table-column>
    <el-table-column
      prop="productQuoteSkill"
      label="productQuoteSkill"
      width="160"
    ></el-table-column>
    <el-table-column
      prop="gpoSecondQuoteSkill"
      label="gpoSecondQuoteSkill"
      width="170"
    ></el-table-column>
    <el-table-column
      prop="contractQuoteSkill"
      label="contractQuoteSkill"
      width="160"
    ></el-table-column>
    <el-table-column
      prop="masterRegistrationSkill"
      label="masterRegistrationSkill"
      width="180"
    ></el-table-column>
    <el-table-column
      prop="certificateSkill"
      label="certificateSkill"
      width="160"
    ></el-table-column>
    <el-table-column
      prop="msaPaymentSkill"
      label="msaPaymentSkill"
      width="160"
    ></el-table-column>
    <el-table-column
      prop="postDiscountSkill"
      label="postDiscountSkill"
      width="160"
    ></el-table-column>
    <el-table-column
      prop="sampleArrangementSkill"
      label="sampleArrangementSkill"
      width="180"
    ></el-table-column>
    <el-table-column
      prop="shortTermLoanSkill"
      label="shortTermLoanSkill"
      width="160"
    ></el-table-column>
    <el-table-column
      prop="inventoryPromotionSkill"
      label="inventoryPromotionSkill"
      width="180"
    ></el-table-column>
    <el-table-column
      prop="loanerEquipmentSkill"
      label="loanerEquipmentSkill"
      width="160"
    ></el-table-column>
    <el-table-column
      prop="equipmentArrangementSkill"
      label="equipmentArrangementSkill"
      width="200"
    ></el-table-column>
    <el-table-column
      prop="indirectSalesCaseSkill"
      label="indirectSalesCaseSkill"
      width="200"
    ></el-table-column>
    <el-table-column
      prop="newSCCaseSkill"
      label="newSCCaseSkill"
      width="160"
    ></el-table-column>
    <el-table-column
      prop="directSalesCaseSkill"
      label="directSalesCaseSkill"
      width="160"
    ></el-table-column>
    <el-table-column
      prop="japaneseCertificate"
      label="japaneseCertificate"
      width="160"
    ></el-table-column>
    <el-table-column
      prop="japaneseAbility"
      label="japaneseAbility"
      width="160"
    ></el-table-column>
    <el-table-column
      prop="studyAbroadExperience"
      label="studyAbroadExperience"
      width="180"
    ></el-table-column>
    <el-table-column
      prop="englishLevel"
      label="englishLevel"
      width="110"
    ></el-table-column>
    <el-table-column
      prop="callHandlingExperience"
      label="callHandlingExperience"
      width="200"
    ></el-table-column>
    <el-table-column
      prop="ktExperience"
      label="ktExperience"
      width="110"
    ></el-table-column>
    <el-table-column
      prop="domainExperience1"
      label="domainExperience1"
      width="180"
    ></el-table-column>
    <el-table-column
      prop="domainExperience2"
      label="domainExperience2"
      width="180"
    ></el-table-column>
    <el-table-column
      prop="excelSkill"
      label="excelSkill"
      width="100"
    ></el-table-column>
    <el-table-column
      prop="otherQualifications"
      label="otherQualifications"
      width="150"
    ></el-table-column>
    <el-table-column
      prop="otherSkills"
      label="otherSkills"
      width="150"
    ></el-table-column>
  </el-table>
  <el-table
    v-if="importType === 2 && importSuccess"
    :data="tableData"
    border
    style="width: 100%"
  >
    <el-table-column prop="id" label="id"></el-table-column>
    <el-table-column prop="guid" label="guid"></el-table-column>
    <el-table-column prop="employeeId" label="employeeId"></el-table-column>
    <el-table-column prop="userName" label="userName"></el-table-column>
    <el-table-column prop="leaveType" label="leaveType"></el-table-column>
    <el-table-column
      prop="attendanceYear"
      label="attendanceYear"
    ></el-table-column>
    <el-table-column
      prop="attendanceMonth"
      label="attendanceMonth"
    ></el-table-column>
    <el-table-column
      prop="attendanceDay"
      label="attendanceDay"
    ></el-table-column>
    <el-table-column prop="leaveDays" label="leaveDays"></el-table-column>
  </el-table>
  <el-table
    v-if="importType === 3 && importSuccess"
    :data="tableData"
    border
    style="width: 100%"
  >
    <el-table-column prop="year" label="year"></el-table-column>
    <el-table-column prop="holidayDate" label="holidayDate"></el-table-column>
    <el-table-column prop="holidayName" label="holidayName"></el-table-column>
  </el-table>
</template>

<script setup lang="ts">
import { UploadFilled, Upload } from "@element-plus/icons-vue";

const upload = ref<UploadInstance>();
import { ref, onMounted } from "vue";

const currentRow = ref<any>();
const tableData = ref([]); // 表格数据
const loading = ref(false); // 加载状态
const importSuccess = ref(false);
const fileList = ref([]);
const importType = ref();
const importMod = ref();
const charset = ref();
const handleCurrentChange = (val: any | undefined) => {
  currentRow.value = val!;
  console.log("handleCurrentChange", val);
};
const tableColumns = [
  { prop: "FileId", label: "文件ID", width: "160" },
  { prop: "FileName", label: "文件名" },
  { prop: "createDate", label: "创建时间" },
];
import {
  ElMessage,
  genFileId,
  UploadInstance,
  UploadProps,
  UploadRawFile,
} from "element-plus";
import { currentFILE, currentFILEPOST, currentGET } from "@/api";

const fetchData = async () => {
  loading.value = true;
  try {
    const filteredData = await currentGET("GetPermissionManagement", {});
    tableData.value = filteredData;
  } catch (error) {
    console.error("数据加载失败", error);
  } finally {
    loading.value = false;
  }
};

// 初始化加载数据
// onMounted(() => {
//   fetchData();
// });

const handleExceed: UploadProps["onExceed"] = (files) => {
  upload.value!.clearFiles();
  const file = files[0] as UploadRawFile;
  file.uid = genFileId();
  upload.value!.handleStart(file);
};
const doUpload = async () => {
  let apiKey;
  if (importType.value == 1) {
    apiKey = "ImportSkillMatrixData";
  } else if (importType.value == 2) {
    apiKey = "ImportEmployeeAttendanceData";
  } else if (importType.value == 3) {
    apiKey = "ImportJapanHolidaysData";
  }
  if (!apiKey) {
    return;
  }
  const files: any[] = fileList.value;
  if (files && files.length > 0) {
    const formData = new FormData();
    formData.append("file", files[0].raw);
    loading.value = true;
    try {
      tableData.value = [];
      importSuccess.value = false;
      const res = await currentFILEPOST(apiKey, formData);
      if (res.code === 0) {
        ElMessage({
          showClose: true,
          message: "导入成功",
          type: "success",
        });
        fileList.value = [];
        tableData.value = res.result;
        importSuccess.value = true;
      } else {
        ElMessage({
          showClose: true,
          message: "导入失败",
          type: "warning",
        });
      }
    } finally {
      loading.value = false;
    }
  }
};
</script>

<style scoped></style>
