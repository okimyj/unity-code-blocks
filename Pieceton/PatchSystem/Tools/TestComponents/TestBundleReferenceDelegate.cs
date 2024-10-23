#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace Pieceton.PatchSystem
{
    public abstract partial class TestBundleReference
    {
        protected delegate void CheckFunction(string _bundle_name, int _target_ref_count);

        protected delegate void SubFunction();
        protected delegate IEnumerator SubFunctionIEnum();

        protected delegate void SubFunctionArg(params object[] _args);
        protected delegate IEnumerator SubFunctionIEnumArg(params object[] _args);

        protected void Do(SubFunction _func)
        {
            AddDoingList(_func.Method);
            _func();
        }

        protected IEnumerator IEnumDo(SubFunctionIEnum _func)
        {
            AddDoingList(_func.Method);
            yield return _func();
        }

        protected void DoArg(SubFunctionArg _func, params object[] _args)
        {
            AddDoingList(_func.Method);
            _func(_args);
        }

        protected IEnumerator IENumArgDo(SubFunctionIEnumArg _func, params object[] _args)
        {
            AddDoingList(_func.Method);
            yield return _func(_args);
        }
    }
}
#endif //UNITY_EDITOR