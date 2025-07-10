import { statusOption } from "@/views/ScreenDesign2/component/consts";
import moment from "moment";
import { saveAs } from 'file-saver'
import { number } from "echarts";
import { SystemSetting, SystemSettingModel } from "@/api/models";

export * from "./storage";

export const dateTimeFmt = (_row: any, _column: any, val: any) => {
  if (val) {
    return moment(val).format("YYYY-MM-DD HH:mm:ss");
  }
  return "";
};
export const ynFmt = (_row: any, _column: any, val: any) => {
  if (typeof val === 'number') {
    return val ? '是' : '否';
  }
  return "";
};
export const statusFmt = (_row: any, _column: any, val: any) => {
  const statusItem = statusOption.find(
    (item) => `${item.value}` === `${val}`
  );
  return statusItem ? statusItem.label : "未知状态";
};
export const dateTimeFormat = (val: any) => {
  if (val) {
    return moment(val).format("YYYY-MM-DD HH:mm:ss");
  }
  return "";
};

export function extractFilename(contentDisposition?: string) {
  if (!contentDisposition) return 'downloaded_file';
  
  // 尝试匹配 UTF-8 编码的文件名（filename*=UTF-8''...）
  const utf8Match = contentDisposition.match(/filename\*=UTF-8''([^;]+)/i);
  if (utf8Match && utf8Match[1]) {
    try {
      // 解码 URI 组件（处理特殊字符）
      return decodeURIComponent(utf8Match[1]);
    } catch (e) {
      console.warn('解码 UTF-8 文件名失败:', e);
    }
  }
  return 'downloaded_file'
}

export function saveFile(data: any) {
  // 创建 Blob 对象
  // const uint8Array = new TextEncoder().encode(data.data);
  const blob = new Blob([data.data], { type: data.headers["content-type"] });
  // 从响应头中获取文件名（需后端配合）

  const fileName = extractFilename(data.headers["content-disposition"]);
  saveAs(blob, fileName)
  // // 创建下载链接
  // const urlObject = window.URL || window.webkitURL || window;
  // const blobUrl = urlObject.createObjectURL(blob);

  // // 创建a标签并触发下载
  // const link = document.createElement("a");
  // link.href = blobUrl;
  // link.setAttribute("download", fileName);
  // document.body.appendChild(link);
  // link.click();
  // // 清理
  // document.body.removeChild(link);
  // window.URL.revokeObjectURL(blobUrl);
}

export function getSystemSetting(systemSettings?: SystemSetting[]): SystemSettingModel {
  const res: SystemSettingModel = {
    standard_maker_time: 0,
    standard_checker_time: 0
  }
  if (systemSettings && systemSettings.length > 0) {
    for (const s of systemSettings) {
      if (s.SettingKey === 'standard_maker_time') {
        res.standard_maker_time = Number(s.SettingValue)
        continue
      }
      if (s.SettingKey === 'standard_checker_time') {
        res.standard_checker_time = Number(s.SettingValue)
      }
    }
  }
  return res;
}