<script setup lang="ts">
import { reactive, ref } from "vue";
import type { FormInstance } from "element-plus";
import { ElMessage } from "element-plus";
import { POST, POST2 } from "@/api/api";
import { getLocalStorage, setLocalStorage } from "@/utils";
import { StorageEnum } from "@/enums";
import { useRouter } from "vue-router";
import { backends } from '@/views/ScreenDesign2/component/consts'

const router = useRouter();
const formRef = ref<FormInstance>();
const dynamicValidateForm = reactive<{
  guid: string;
  password: string;
}>({
  guid: "",
  password: "",
});
const backend = ref<string>(getLocalStorage(StorageEnum.BACKEND_NAME))

const submitForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return;
  formEl.validate(async (valid) => {
    if (valid) {
      const token = await POST2("/api/auth/token", dynamicValidateForm);
      if (token && token.code == 0) {
        setLocalStorage(StorageEnum.GB_TOKEN_STORE, token.result);
        router.push("/");
      } else {
        ElMessage.error("用户名密码错误");
      }
    }
  });
};
</script>

<template>
  <div class="main-container">
    <div class="w-96 mx-auto py-8 px-8 bg-white rounded-md relative top-1/3">
      <el-form
        ref="formRef"
        :model="dynamicValidateForm"
        label-width="6rem"
        class="demo-dynamic"
      >
        <el-row>
          <el-col>
            <el-form-item prop="backend" label="直販/間販">
              <el-select
                v-model="backend"
                style="width: 100%"
                placeholder=""
                @change="(val) => setLocalStorage(StorageEnum.BACKEND_NAME, val)"
              >
                <el-option
                  v-for="item in backends"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col>
            <el-form-item
              prop="guid"
              label="用户名"
              :rules="[
                {
                  required: true,
                  message: '输入用户名',
                  trigger: ['blur', 'change'],
                },
              ]"
            >
              <el-input
                v-model="dynamicValidateForm.guid"
                placeholder="输入用户名"
              />
            </el-form-item>
          </el-col>
          <el-col>
            <el-form-item
              prop="password"
              label="密码"
              :rules="[
                {
                  required: true,
                  message: '输入密码',
                  trigger: ['blur', 'change'],
                },
              ]"
            >
              <el-input
                v-model="dynamicValidateForm.password"
                type="password"
                show-password
                placeholder="输入密码"
              />
            </el-form-item>
          </el-col>
          <el-col class="pl-24">
            <el-button type="primary" @click="submitForm(formRef)"
              >登录</el-button
            >
          </el-col>
        </el-row>
      </el-form>
    </div>
  </div>
</template>

<style scoped lang="scss">
.main-container {
  width: 100%;
  height: 100%;
  background: url("@/assets/img/login_bg.jpg") no-repeat;
  background-size: cover;
}
button {
  background-color: var(--el-color-primary);
}
</style>
