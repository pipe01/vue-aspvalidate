import http from "axios";
export default function VueAspValidate(Vue, options) {
  Vue.component("asp-form", {
    data() {
      return {
        funcs: null
      };
    },

    render() {
      const h = arguments[0];
      return h("form", {
        "ref": "frm",
        "on": {
          "submit": this.submit
        }
      }, [this.$slots.default]);
    },

    async created() {
      var formId = this.$attrs["id"];
      var resp = await http.get("/vaspval/" + formId + ".js");
      this.funcs = eval(resp.data);
    },

    methods: {
      submit(e) {
        e.preventDefault();
        var errors = {};
        var formVue = this.$refs["frm"];
        var form = formVue;

        for (const item of form) {
          var input = item;

          if (input.name != null && this.funcs[input.name] != null) {
            var validators = this.funcs[input.name];

            for (const val of validators) {
              var result = val(input.value);

              if (result !== true) {
                var errorMsg = "Invalid format";
                if (typeof result === "string") errorMsg = result.replace("{0}", input.name);
                var inputErrors = errors[input.name];
                if (inputErrors == null) inputErrors = errors[input.name] = [];
                inputErrors.push(errorMsg);
              }
            }
          }
        }

        for (const item of form.querySelectorAll("*")) {
          var forName = item.getAttribute("for");
          if (forName == null) continue;

          if (errors[forName] != null) {
            item.innerHTML = errors[forName].join("<br>");
          }
        }
      }

    }
  });
}