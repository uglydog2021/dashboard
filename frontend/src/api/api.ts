/*
 * @LastEditors: Please set LastEditors
 * @LastEditTime: 2023-05-08 17:44:36
 */
import axios from "axios";

import type { AxiosRequestConfig, AxiosResponse } from "axios";
import { StorageEnum, RequestEnum } from "@/enums";
import { getLocalStorage, setLocalStorage } from "@/utils";

import UtilVar from "../config/UtilVar";
import router from "@/router";
import { reactive } from "vue";
const availableBackends = UtilVar.availableBackends;
const CancelToken = axios.CancelToken;

const getBaseUrl = (name?: string) => {
  if (!availableBackends) {
    return UtilVar.baseUrl;
  }
  return (
    availableBackends[name ? name : getLocalStorage(StorageEnum.BACKEND_NAME)] ??
    UtilVar.baseUrl
  );
};
// axios.defaults.withCredentials = true;
// 添加请求拦截器
axios.interceptors.request.use(
  function (config: AxiosRequestConfig): any {
    // 在发送请求之前做些什么 传token
    let token: any = getLocalStorage(StorageEnum.GB_TOKEN_STORE);
    if (token) {
      // @ts-ignore
      config.headers[RequestEnum.GB_TOKEN_KEY] = "Bearer " + token;
    }
    // @ts-ignore
    if (!config.headers["Content-Type"]) {
      // @ts-ignore
      config.headers["Content-Type"] = "application/json;charset=utf-8";
    }

    return config;
  },
  function (error: any) {
    // 对请求错误做些什么
    console.log(error);
    return Promise.reject(error);
  }
);

export type Params = { [key: string]: string | number };
export type FileConfig = {
  setCancel?: Function;
  onProgress?: Function;
  [key: string]: any;
};
// 是否正在刷新的标记
let isRefreshing = false;
// 重试队列，每一项将是一个待执行的函数形式
let requests: (() => void)[] = [];
/**
 * @响应拦截
 */
axios.interceptors.response.use(
  (response: AxiosResponse) => {
    if (response.status !== 200) {
      return Promise.reject(response);
    }
    if (response.data.code == UtilVar.code) {
      // router.push("/login")
      return response.data;
    }
    if (response.config.responseType === 'blob' || 'arraybuffer' === response.config.responseType) {
        return response
    }
    return response.data;
  },
  (error: any) => {
    if (
      error.status === 401 &&
      error.config.url !== getBaseUrl() + "/api/auth/refresh-token"
    ) {
      if (!isRefreshing) {
        isRefreshing = true;
        return refresh()
          .then((res) => {
            setLocalStorage(StorageEnum.GB_TOKEN_STORE, res.result);
            // 已经刷新了token，将所有队列中的请求进行重试
            requests.forEach((cb) => cb());
            requests = [];
            return axios(error.config);
          })
          .catch((res) => {
            console.error("refreshtoken error =>", res);
            router.push({ name: "Login" });
          })
          .finally(() => {
            isRefreshing = false;
          });
      } else {
        // 正在刷新token，将返回一个未执行resolve的promise
        return new Promise((resolve) => {
          // 将resolve放进队列，用一个函数形式来保存，等token刷新后直接执行
          requests.push(() => {
            resolve(axios(error.config));
          });
        });
      }
    }
    if (error.status === 403 || error.status === 401) {
      router.push({ name: "Login" });
    }
    console.error(error);
    return Promise.reject(error);
  }
);

async function refresh() {
  let token: any = getLocalStorage(StorageEnum.GB_TOKEN_STORE);
  return await POST("/api/auth/refresh-token", { token });
}

//判断是否是加密参数，是的话处理
let isEncryptionParam = (params: Params) => {
  return params;
};
/**
 * @description: get 请求方法
 * @param {string} url 请求地址
 * @param {Params} params 请求参数
 * @return {*}
 */
export const GET = async (url: string, params: Params): Promise<any> => {
  try {
    params = isEncryptionParam(params);
    const data = await axios.get(`${getBaseUrl()}${url}`, {
      params: params,
    });
    return data;
  } catch (error) {
    return error;
  }
};
export const GETByName = async (url: string, params: Params, connectionName?: string): Promise<any> => {
  try {
    params = isEncryptionParam(params);
    const data = await axios.get(`${getBaseUrl(connectionName)}${url}`, {
      params: params,
    });
    return data;
  } catch (error) {
    return error;
  }
};
/**
 * @description: post请求方法
 * @param {any} url
 * @param {any} params
 * @return {any}
 */
export const POST = async (url: string, params: Params): Promise<any> => {
  params = isEncryptionParam(params);
  return await axios.post(`${getBaseUrl()}${url}`, params);
};
export const POST2 = async (url: string, params: Params): Promise<any> => {
  params = isEncryptionParam(params);
  return await axios.post(`${getBaseUrl()}${url}`, null, {
    params,
  });
};
/**
 * @description: 没有基地址 访问根目录下文件
 * @param {string} url
 * @param {Params} params
 * @return {*}
 */
export const GETNOBASE = async (url: string, params?: Params): Promise<any> => {
  try {
    const data = await axios.get(url, {
      params: params,
    });
    return data;
  } catch (error) {
    return error;
  }
};

// 定义文件类型提交方法
interface fileconfigs {
  [headers: string]: {
    "Content-Type": string;
  };
}
let configs: fileconfigs = {
  headers: { "Content-Type": "multipart/form-data" },
};
/**
 * @description: @文件类型提交方法
 * @param {string} url
 * @param {Params} params
 * @param {FileConfig} config
 * @return {*}
 */
export const FILEPOST = async (
  url: string,
  params: Params,
  config: FileConfig = {}
): Promise<any> => {
  try {
    const data = await axios.post(`${getBaseUrl()}${url}`, params, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    return data;
  } catch (err) {
    return err;
  }
};

/**
 * 下载文档流
 * @param {config.responseType} 下载文件流根据后端 配置   arraybuffer || blod
 */
export const FILE = async (config: FileConfig = {}) => {
  const data = await axios({
    method: config.method || "get",
    url: `${getBaseUrl()}${config.url}`,
    data: config.body || {},
    params: config.param || {},
    responseType: config.responseType || "blob",
    onDownloadProgress: (e: any) => {
      // console.log(e,e.currentTarget)
      // if (e.currentTarget.response.size > 0) {
      //     e.percent = e.loaded / e.currentTarget.response.size * 100;
      // }
      // event.srcElement.getResponseHeader('content-length')
      config.onProgress && config.onProgress(e);
    },
  });
  return data;
};
export const FILEDOWN = async (config: FileConfig = {}) => {
  const data = await axios.post(
    `${getBaseUrl()}${config.url}`,
    config.body,
    {
      responseType: config.responseType || "blob",
      onDownloadProgress: (e: any) => {
        // console.log(e,e.currentTarget)
        // if (e.currentTarget.response.size > 0) {
        //     e.percent = e.loaded / e.currentTarget.response.size * 100;
        // }
        // event.srcElement.getResponseHeader('content-length')
        config.onProgress && config.onProgress(e);
      },
    }
  );
  return data;
};

export const PUT = async (url: string, params: Params) => {
  try {
    params = isEncryptionParam(params);
    const data = await axios.put(`${getBaseUrl()}${url}`, params);
    return data;
  } catch (error) {
    return error;
  }
};
export const DELETE = async (url: string, params: Params) => {
  // console.log(params)
  try {
    params = isEncryptionParam(params);
    const data = await axios.delete(`${getBaseUrl()}${url}`, { data: params });
    return data;
  } catch (error) {
    return error;
  }
};

// switch (error.response?.status) {
//     case 400:
//       error.message = '请求错误(400)';
//       break;
//     case 401:
//       error.message = '未授权(401)';
//       break;
//     case 403:
//       error.message = '拒绝访问(403)';
//       break;
//     case 404:
//       error.message = '请求出错(404)';
//       break;
//     case 408:
//       error.message = '请求超时(408)';
//       break;
//     case 500:
//       error.message = '服务器错误(500)';
//       break;
//     case 501:
//       error.message = '服务未实现(501)';
//       break;
//     case 502:
//       error.message = '网络错误(502)';
//       break;
//     case 503:
//       error.message = '服务不可用(503)';
//       break;
//     case 504:
//       error.message = '网络超时(504)';
//       break;
//     case 505:
//       error.message = 'HTTP版本不受支持(505)';
//       break;
//     default:
//       error.message = `连接出错(${error.response?.status})!`;
//   }
