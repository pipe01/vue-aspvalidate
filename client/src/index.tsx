import _Vue from "vue"
import http from "axios"

export default function VueAspValidate(Vue: typeof _Vue, options?: any): void {
    Vue.component("asp-form", {
        data() {
            return {
                funcs: null as any as { [name: string]: ((value: any) => (boolean | string))[] }
            }
        },
        render() {
            return (
                <form ref="frm" onSubmit={this.submit}>
                    { this.$slots.default }
                </form>
            )
        },
        async created() {
            var formId = this.$attrs["id"];

            var resp = await http.get("/vaspval/" + formId + ".js");
            this.funcs = eval(resp.data)
        },
        methods: {
            submit(e) {
                e.preventDefault();

                var errors: { [name: string]: string[] } = {};
                var formVue = this.$refs["frm"];
                var form = formVue as HTMLFormElement;
                
                for (const item of form) {
                    var input = item as HTMLInputElement;

                    if (input.name != null && this.funcs[input.name] != null) {
                        var validators = this.funcs[input.name];
                        
                        for (const val of validators) {
                            var result = val(input.value);

                            if (result !== true) {
                                var errorMsg = (result as string).replace("{0}", input.name);

                                var inputErrors = errors[input.name];

                                if (inputErrors == null)
                                    inputErrors = errors[input.name] = [];

                                inputErrors.push(errorMsg);
                            }
                        }
                    }
                }

                for (const item of form.querySelectorAll("*")) {
                    var forName = item.getAttribute("for");

                    if (forName == null)
                        continue;

                    if (errors[forName] != null) {
                        item.innerHTML = errors[forName].join("<br>");
                    }
                }
            }
        }
    });
}