<template>
  <div class="container">
    <el-dialog :title="textMap[status]" :visible.sync="visiable">
      
      <el-form :rules="rules" ref="dataForm" :model="temp" label-position="right" label-width="90px" style='width: 600px; margin-left:10px;'>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="字段名" prop="columnName">
              <el-input placeholder="Please input" v-model="temp.columnName">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="中文名称" prop="columnCNName">
              <el-input placeholder="Please input" v-model="temp.columnCNName">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="10">
          <el-col :span="12">
            <el-form-item label="数据类型" prop="columnType">
              <el-select v-model="selVal" filterable allow-create placeholder="请选择" @change="OnElChange">
                <el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value">
                </el-option>
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="6" v-if="temp.columnType.indexOf('numeric') >= 0">
            <el-form-item label="字节长度" prop="byteLength">
              <el-input placeholder="Please input" v-model.number="temp.byteLength">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="6" v-if="temp.columnType.indexOf('varchar') >= 0">
            <el-form-item label="字符长度" prop="charLength">
              <el-input placeholder="Please input" v-model.number="temp.charLength">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="6" v-if="temp.columnType.indexOf('numeric') >= 0">
            <el-form-item label="精度" prop="scale">
              <el-input placeholder="Please input" v-model.number="temp.scale">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="默认值" prop="defaultValue">
              <el-input placeholder="Please input" v-model="temp.defaultValue">
              </el-input>

            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="允许空" prop="isNullable">
              <el-switch v-model="temp.isNullable">
              </el-switch>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="主键" prop="isPrimaryKey">
              <el-switch v-model="temp.isPrimaryKey">
              </el-switch>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="自增" prop="isIdentity">
              <el-switch v-model="temp.isIdentity">
              </el-switch>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="注释" prop="remark">
          <el-input type="textarea" :autosize="{ minRows: 4, maxRows: 8}" placeholder="Please input" v-model="temp.remark">
          </el-input>
        </el-form-item>

      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="cancel">{{$t('table.cancel')}}</el-button>
        <el-button v-if="status=='create'" type="primary" @click="createData">{{$t('table.confirm')}}</el-button>
        <el-button v-else type="primary" @click="updateData">{{$t('table.confirm')}}</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import {
  fetchColumnList,
  createArticle,
  updateArticle,
  fetchModelById
} from "@/api/article";

export default {
  name: "CreateOrUpdate",
  props: {
    formId: {
      type: String,
      default: null
    },
    formPId: {
      type: String,
      default: null
    },
    formStatus: {
      type: String,
      default: ""
    },
    formVisiable: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      temp: {
        id: "",
        dbId: "",
        columnName: "",
        columnCNName: "",
        columnType: "",
        byteLength: -1,
        charLength: -1,
        scale: -1,
        defaultValue: "",
        remark: "",
        isNullable: false,
        isPrimaryKey: false,
        isIdentity: false
      },
      textMap: {
        update: "编辑",
        create: "新增"
      },
      options: [
        {
          value: "int",
          label: "int"
        },
        {
          value: "nvarchar(50)",
          label: "nvarchar(50)"
        },
        {
          value: "bit",
          label: "bit"
        },
        {
          value: "numeric(18,0)",
          label: "numeric(18,0)"
        },
        {
          value: "uniqueidentifier",
          label: "uniqueidentifier"
        }
      ],
      rules: {
        columnName: [
          { required: true, message: "请输入字段名", trigger: "blur" },
          { max: 50, message: "长度在50 个字符以内", trigger: "blur" }
        ],
        columnCNName: [
          { required: true, message: "请输入中文名称", trigger: "blur" },
          { max: 50, message: "长度在50 个字符以内", trigger: "blur" }
        ],
        columnType: [
          { required: true, message: "请选择数据类型", trigger: "blur" }
        ],
        byteLength: [
          { required: true, type: "number", message: "字节长度必须为数字值" }
        ],
        charLength: [
          { required: true, type: "number", message: "字符长度必须为数字值" }
        ],
        scale: [{ required: true, type: "number", message: "精度必须为数字值" }]
      }
    };
  },
  computed: {
    status() {
      if (this.formStatus == "create") {
      }
      if (this.formStatus == "update") {
        fetchModelById({
          Id: this.formId
        }).then(response => {
          this.temp = response.data.result;
        });
      }

      return this.formStatus;
    },
    visiable: {
      get: function() {
        if (this.formVisiable) {
          this.$nextTick(() => {
            this.$refs["dataForm"].resetFields();
            this.temp.dbId = this.formPId;
          });
        }
        return this.formVisiable;
      },
      set: function(val) {
        this.cancel();
      }
    },
    selVal: {
      get: function() {
        if (this.temp.columnType && this.temp.columnType != "") {
          if (this.temp.charLength >= 0) {
            return this.temp.columnType + "(" + this.temp.charLength + ")";
          } else if (this.temp.byteLength >= 0) {
            return (
              this.temp.columnType +
              "(" +
              this.temp.byteLength +
              "," +
              this.temp.scale +
              ")"
            );
          }
        }
        return this.temp.columnType;
      },
      set: function(val) {
        if (val.indexOf("(") >= 0) {
          this.temp.columnType = val.substr(0, val.indexOf("("));
        } else {
          this.temp.columnType = val;
        }
      }
    }
  },
  methods: {
    createData() {
      this.$refs["dataForm"].validate(valid => {
        if (valid) {
          createArticle(this.temp).then(() => {
            this.$emit("cu-visiable-change", false, "", this.temp);
            this.$message({
              type: "success",
              message: "创建成功!"
            });
          });
        }
      });
    },
    updateData() {
      this.$refs["dataForm"].validate(valid => {
        if (valid) {
          updateArticle(this.temp).then(() => {
            this.$emit("cu-visiable-change", false, "", this.temp);
            this.$message({
              type: "success",
              message: "更新成功!"
            });
          });
        }
      });
    },
    cancel: function() {
      this.$confirm("是否放弃数据的修改?", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning"
      })
        .then(() => {
          if (this.formStatus == "create") {
            this.$message({
              type: "info",
              message: "已取消新增"
            });
            this.$emit("cu-visiable-change", false, "cancel", this.temp);
          } else if (this.formStatus == "update") {
            this.$message({
              type: "info",
              message: "已取消修改"
            });
          }
          this.$emit("cu-visiable-change", false, "cancel", this.temp);
        })
        .catch(() => {});
    },
    OnElChange: function(val) {
      this.temp.charLength = -1;
      this.temp.byteLength = -1;
      this.scale = -1;

      var pattern = /\(([^()]+)\)/g;
      if (val.indexOf("(") >= 0) {
        var arrResult = pattern.exec(val);
        var result = arrResult[1];
        if (val.indexOf("varchar") >= 0) {
          this.temp.charLength = parseInt(result);
        } else if (val.indexOf("numeric") >= 0) {
          var arr = result.split(",");
          this.temp.byteLength = parseInt(arr[0]);
          this.temp.scale = parseInt(arr[1]);
        }
      }
    }
  }
};
</script>