using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerCompiler
{
    /// <summary>
    /// 開発対象。
    /// </summary>
    [Serializable]
    public enum DevelopmentTarget
    {
        /// <summary>指定無し。</summary>
        None,
        /// <summary>Windows x86。</summary>
        WindowsX86,
        /// <summary>Windows x64。</summary>
        WindowsX64,
        /// <summary>Linux x86。</summary>
        LinuxX86,
        /// <summary>Linux x64。</summary>
        LinuxX64,
        /// <summary>Linux ARM。</summary>
        LinuxARM,
        /// <summary>Linux ARM64。</summary>
        LinuxARM64,
        /// <summary>macOS x64。</summary>
        macOSX64,
    }

    /// <summary>
    /// 開発環境。
    /// </summary>
    [Serializable]
    public enum DevelopmentEnvironment
    {
        /// <summary>指定無し。</summary>
        None,
        /// <summary>Visual Studio。</summary>
        VisualStudio,
        /// <summary>GCC。</summary>
        GCC,
        /// <summary>Clang。</summary>
        Clang,
    }

}
