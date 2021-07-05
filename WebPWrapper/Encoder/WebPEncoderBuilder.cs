﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;

namespace WebPWrapper.Encoder {
    /// <summary>
    /// 
    /// </summary>
    public class WebPEncoderBuilder : IWebPEncoderBuilder {
        /// <summary>
        /// 建構中的參數暫存
        /// </summary>
        private List<(string key, string value)> _arguments = new List<(string key, string value)>();

        private string _executeFilePath;

        /// <summary>
        /// Windows下的目录名
        /// </summary>
        public const string _windowsDir = "libwebp-1.0.2-windows-x86";

        /// <summary>
        /// Linux下的目录名
        /// </summary>
        public const string _linuxDir = "libwebp-1.0.2-linux-x86-64";

        /// <summary>
        /// OSX下的用户名
        /// </summary>
        public const string _osxDir = "libwebp-1.0.2-mac-10.14";


        /// <summary>
        /// 初始化WebP編碼器建構器
        /// </summary>
        /// <param name="executeFilePath">執行檔路徑，如為空則使用預設路徑</param>
        public WebPEncoderBuilder(string executeFilePath = null) {
            _executeFilePath = executeFilePath;
            if (_executeFilePath == null) {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    _executeFilePath = $"webp/{_windowsDir}/bin/cwebp.exe";
                } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    _executeFilePath = $"webp/{_linuxDir}/bin/cwebp";
                } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                    _executeFilePath = $"webp/{_osxDir}/bin/cwebp";
                } else {
                    throw new PlatformNotSupportedException();
                }
                _executeFilePath = Path.Combine(Path.GetFullPath("."), _executeFilePath);
            }

            if (!File.Exists(_executeFilePath)) {
                throw new FileNotFoundException();
            }
        }

        /// <summary>
        /// 輸入圖片裁減
        /// </summary>
        /// <param name="x">起始座標X</param>
        /// <param name="y">起始座標Y</param>
        /// <param name="width">寬度</param>
        /// <param name="height">高度</param> 
        public IWebPEncoderBuilder Crop(int x, int y, int width, int height) {
            _arguments.Add((key: "-crop", value: $"{x} {y} {width} {height}"));
            return this;
        }

        /// <summary>
        /// 縮放圖片，<paramref name="height"/>與<paramref name="width"/>至少一者非0，如果其中一值為0則等比例縮放
        /// </summary>
        /// <param name="width">寬度</param>
        /// <param name="height">寬度</param> 
        public IWebPEncoderBuilder Resize(int width, int height) {
            _arguments.Add((key: "-resize", $"{width} {height}"));
            return this;
        }

        /// <summary>
        /// 容許多執行序
        /// </summary>
        public IWebPEncoderBuilder MultiThread() {
            _arguments.Add((key: "-mt", value: null));
            return this;
        }

        /// <summary>
        /// 降低記憶體使用
        /// </summary>
        public IWebPEncoderBuilder LowMemory() {
            _arguments.Add((key: "-low_memory", value: null));
            return this;
        }

        /// <summary>
        /// 複製來源圖片的Metadata
        /// </summary>
        public IWebPEncoderBuilder CopyMetadata(params Metadatas[] metadatas) {
            if (metadatas == null || metadatas.Length == 0) {
                throw new ArgumentNullException(nameof(metadatas));
            }
            _arguments.Add((key: "-metadata", value: string.Join(",", metadatas.Select(x => x.ToString().ToLower()))));
            return this;
        }

        /// <summary>
        /// 停用ASM優化
        /// </summary>
        public IWebPEncoderBuilder DisableAssemblyOptimization() {
            _arguments.Add((key: "-noasm", value: null));
            return this;
        }

        /// <summary>
        /// 讀取預設的組態
        /// </summary>
        /// <param name="profile">組態類型</param> 
        public IWebPEncoderBuilder LoadPresetProfile(PresetProfiles profile) {
            _arguments.Add((key: "-preset", value: profile.ToString().ToLower()));
            return this;
        }

        /// <summary>
        /// 設定壓縮組態
        /// </summary>
        /// <param name="config">壓縮組態設定</param> 
        public IWebPEncoderBuilder CompressionConfig(Expression<Action<CompressionConfiguration>> config) {
            var _compressionConfiguration = new CompressionConfiguration();
            config.Compile().Invoke(_compressionConfiguration);

            _arguments.Add((key: nameof(CompressionConfig), value: _compressionConfiguration.GetCurrentArguments()));
            return this;
        }

        /// <summary>
        /// 設定Alpha組態
        /// </summary>
        /// <param name="config">Alpha組態設定</param>
        public IWebPEncoderBuilder AlphaConfig(Expression<Action<AlphaConfiguration>> config) {
            var _alphaConfiguration = new AlphaConfiguration();
            config.Compile().Invoke(_alphaConfiguration);

            _arguments.Add((key: nameof(AlphaConfig), value: _alphaConfiguration.GetCurrentArguments()));
            return this;
        }

        /// <summary>
        /// 重設回預設值
        /// </summary>
        public IWebPEncoderBuilder Reset() {
            _arguments.Clear();
            return this;
        }

        /// <summary>
        /// 建構WebP編碼器
        /// </summary>
        /// <returns>WebP編碼器</returns>
        public IWebPEncoder Build() {
            var args = GetCurrentArguments();
            return new WebPEncoder(_executeFilePath, args);
        }

        /// <summary>
        /// 取得目前CLI參數
        /// </summary>
        /// <returns>CLI參數</returns>
        public string GetCurrentArguments() {
            return string.Join(" ", _arguments.Select(x => {
                if (x.key.StartsWith("-")) {
                    return $"{x.key} {x.value}";
                } else {
                    return x.value;
                }
            }));
        }
    }
}
