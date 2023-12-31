﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using XmlCake.String;

namespace Pandora.Patch.Patchers.Skyrim.Hkx
{
	public partial class PackFileEditor
	{
		static private readonly char[] trimChars = new char[3] { '\n', '\r', '\t' };
		public static string GetTrimElementValue(XElement element)
		{
			return element.Value.TrimEnd(trimChars);
		}
		public static XElement ReplaceElement(PackFile packFile, string path, XElement element) => packFile.Map.ReplaceElement(path, element);


		public static string InsertElement(PackFile packFile, string path, XElement element)
		{
			return packFile.Map.InsertElement(path, element, true);
		}

		public static string AppendElement(PackFile packFile, string path, XElement element)
		{
			return packFile.Map.AppendElement(path, element);
		}

		public static string PushElement(PackFile packFile, string path, XElement element)
		{
			return packFile.Map.PushElement(path, element);
		}

		public static XElement RemoveElement(PackFile packFile, string path) => packFile.Map.RemoveElement(path); 

		public static bool ReplaceText(PackFile packFile, string path, string preValue, string oldValue, string newValue)
		{
			
			XElement element = packFile.SafeNavigateTo(path);
			if (String.IsNullOrWhiteSpace(oldValue)) return false;	
			string source = element.Value.Trim().Replace("\t", "").Replace('\n', ' ');
			if (String.IsNullOrWhiteSpace(newValue))
			{

				int index = source.IndexOf(oldValue, StringComparison.Ordinal);
				newValue = (index < 0) ? source : source.Remove(index, oldValue.Length);
				element.SetValue(newValue);
				return true;
			}
			preValue = preValue.Trim().Replace("\t", "").Replace('\n', ' ');
			oldValue = oldValue.Trim().Replace("\t", "").Replace('\n', ' ');
			newValue = newValue.Trim().Replace("\t", "").Replace('\n', ' ');

			int skipCount = preValue.Length > 0 ? Regex.Matches(preValue, oldValue).Count() : 0;
			//if (skipCount == 0) return false;
			int sourceHash = source.GetHashCode();
			int sourceLength = source.Length; 
			
			
			element.SetValue(source.Replace(oldValue, newValue, skipCount+1));
			if ((sourceLength - oldValue.Length + newValue.Length) != element.Value.Length || sourceHash == element.Value.GetHashCode()) 
			{ 
				return false; 
			}
			return true; 

		}
		public static void InsertText(PackFile packFile, string path, string insertAfterValue, string newValue)
		{
			
			XElement element = packFile.SafeNavigateTo(path);
			string source = GetTrimElementValue(element);

			newValue = newValue.Trim().Replace("\t", "").Replace('\n', ' ');

			insertAfterValue = insertAfterValue.Trim().Replace("\t", "").Replace('\n', ' ');

			element.SetValue(source.Insert(insertAfterValue,' ', newValue+Environment.NewLine));
		}

		public static void AppendText(PackFile packFile, string path, string newValue)
		{
			XElement element = packFile.SafeNavigateTo(path);
			string source = GetTrimElementValue(element);

			newValue = newValue.Trim().Replace("\t", "").Replace('\n', ' ');
			element.SetValue(source.Append(' ', newValue+Environment.NewLine));
		}
		public static void RemoveText(PackFile packFile, string path, string value)
		{
			XElement element = packFile.SafeNavigateTo(path);
			if (String.IsNullOrWhiteSpace(value)) return;
			string source = element.Value;
			value = value.Trim().Replace("\t", "").Replace('\n', ' '); 

			element.SetValue(source.Replace(value, string.Empty, true));
		}
	}
}
