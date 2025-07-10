export type OrganizationDO = {
  organizationID?: number;
  organizationName?: string;
  makerTaskCount?: number;
  checkerTaskCount?: number;
};

export interface PermissionDTO {
  guid?: number;
  employeeId?: string;
  user_Name?: string;
  password?: string;
  user_Email?: string;
  organizationID?: string;
  web_Permission?: number;
  data_Permission?: number;
  import_Permission?: number;
  createDate?: string;
  businessType?: string;
}
export type AttentionDO = {
  attention_Id?: number;
  organizationId?: number;
  organizationName?: string;
  start_Time?: string;
  end_Time?: string;
  user_Id?: string;
  subject?: string;
  message?: string;
  createTime?: string;
  updateTime?: string;
};

export interface DataBaseDTO {
  dbName?: string;
  account?: string;
  passWord?: string;
}

export type ResponseData<T> = {
  code: number;
  message: string;
  result: T;
  success: boolean;
  total: number;
};

export type SystemSetting = {
  SettingID: number
  SettingKey: string
  SettingValue: string
  ValueType: string
}

export type SystemSettingModel = {
  standard_maker_time: number
  standard_checker_time: number
}
