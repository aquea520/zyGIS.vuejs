<template>
  <div class="container">
    <el-dialog :title="textMap[status]" :visible.sync="visiable">
      <el-form :rules="rules" ref="dataForm" :model="temp" label-position="left" label-width="70px" style='width: 500px; margin-left:50px;'>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="字段名" prop="columnName">
              <el-input placeholder="Please input" v-model="temp.columnName">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="数据类型" prop="columnType">
              <el-input placeholder="Please input" v-model="temp.columnType">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="字节长度" prop="byteLength">
              <el-input placeholder="Please input" v-model.number="temp.byteLength">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="字符长度" prop="charLength">
              <el-input placeholder="Please input" v-model.number="temp.charLength">
              </el-input>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="精度" prop="scale">
              <el-input placeholder="Please input" v-model.number="temp.scale">
              </el-input>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="默认值" prop="defaultValue">
              <el-input placeholder="Please input" v-model="temp.defaultValue">
              </el-input>

            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="允许空" prop="isNullable">
              <el-switch v-model="temp.isNullable">
              </el-switch>
            </el-form-item>
          </el-col>
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
        dbId: -1,
        columnName: "",
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
      rules: {
        columnName: [
          { required: true, message: "请输入字段名", trigger: "blur" },
          { max: 50, message: "长度在50 个字符以内", trigger: "blur" }
        ],
        byteLength:[
          { type: 'number', message: '字节长度必须为数字值'}
        ],
        charLength:[
          { type: 'number', message: '字符长度必须为数字值'}
        ],
        scale:[
          { type: 'number', message: '精度必须为数字值'}
        ]
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
          });
        }
        return this.formVisiable;
      },
      set: function(val) {
        this.cancel();
      }
    }
  },
  methods: {
    createData() {
      this.$refs["dataForm"].validate(valid => {
        if (valid) {
          createArticle(this.temp).then(() => {
            this.$emit("visiable-change", false, "");
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
            this.$emit("visiable-change", false, "");
            this.$message({
              type: "success",
              message: "更新成功!"
            });
          });
        }
      });
    },
    cancel: function() {
      if (this.formStatus == "create") {
        this.$message({
          type: "info",
          message: "已取消新增"
        });
      } else if (this.formStatus == "update") {
        this.$message({
          type: "info",
          message: "已取消修改"
        });
      }

      this.$emit("visiable-change", false, "cancel");
    }
  }
};
</script>