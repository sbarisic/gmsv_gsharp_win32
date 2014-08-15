function table.Count(tbl) local n = 0 for _ in pairs(tbl) do n = n + 1 end return n end

local function do_print_r(arg,spaces,passed,tf,gf,mf)
	local t = tf(arg);
	if(t == "table") then
		if(arg.r and arg.g and arg.b and arg.a and table.Count(arg) == 4) then mf("Color("..arg.r..","..arg.g..","..arg.b..","..arg.a..")\n"); return; end
		passed[arg] = true; mf("(table) "..tostring(arg):gsub("table: ","").." { \n");
		for k,v in gf(arg) do
			if(not passed[v]) then mf("  "..spaces.."("..tf(k)..") "..tostring(k).." => "); do_print_r(rawget(arg,k),spaces.."  ",passed,tf,gf,mf); else
				mf("  "..spaces.."("..tf(k)..") "..tostring(k).." => [RECURSIVE TABLE: "..tostring(v):gsub("table: ","").."]\n"); end
		end
		mf(spaces.."}\n");
	elseif(t == "function") then mf("("..t..") "..tostring(arg):gsub("function: ","").."\n");
	elseif(t == "string") then mf("("..t..") '"..tostring(arg).."'\n");
	elseif(t == "Vector") then mf(t.."("..arg.x..","..arg.y..","..arg.z..")\n");
	elseif(t == "Angle") then mf(t.."("..arg.p..","..arg.y..","..arg.r..")\n");
	else mf("("..t..") "..tostring(arg).."\n"); end
end

local function print_r(arg, mf, pf, tf)
	mf = mf or Msg; pf = pf or pairs; tf = tf or type; local passed = {}
	do_print_r(arg, "", passed, tf, pf, mf)
end
 
function PrintTable(...)
	local arg = {...}; local passed = {};
	for k = 1, #arg do do_print_r(arg[k], "", passed, type, pairs, Msg); end
end

function Color(R, G, B, A)
	local ClrMeta = {
		__tostring = function()
			return "FAGET"
		end
	}

	local Ret = { r = R or 255, g = G or 255, b = B or 255, a = A or 255 }

	setmetatable(Ret, ClrMeta)
	return Ret
end

C = Color(1)


function T(n)
	Thread.Spawn(function()
		for i = 0, 3 do
			Thread.Print("Hello from thread " .. tostring(n) .. " - " .. tostring(i))
			Thread.Sleep(10);
		end
	end)
end
