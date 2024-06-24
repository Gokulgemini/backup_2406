# Default attributes for packages


default['ec_security_updates']['packages'] = case node['platform']
  when 'ubuntu' then
  [
    {name: 'ghostscript', version: '9.55.0~dfsg1-0ubuntu5.5', action: 'upgrade'},
    {name: 'libgs-dev', version: '9.55.0~dfsg1-0ubuntu5.5', action: 'upgrade'},
    {name: 'imagemagick-6.q16', options: '-f', action: 'upgrade'},
  ]
  when 'centos' then
  [
   {name: 'python', action: 'upgrade'},
   {name: 'unzip', action: 'upgrade'},
   {name: 'ghostscript', action: 'upgrade'},
   {name: 'wkhtmltopdf', options: '-f', action: 'upgrade'},
   {name: 'imagemagick-6.q16', action: 'upgrade'},
   {name: 'nodejs', action: 'upgrade'},
   {name: 'subversion', action: 'remove'},
   {name: 'libX11', action: 'remove'},
   {name: 'libtirpc', action: 'remove'},
   {name: 'apache', action: 'remove'},
   {name: 'libXi', action: 'remove'},
   {name: 'libXfixes', action: 'remove'},
   {name: 'libXpm', action: 'remove'},
   {name: 'libXrender', action: 'remove'},
   {name: 'libICE', action: 'remove'},
   {name: 'libxdmcp6', action: 'remove'},
   {name: 't1lib', action: 'remove'},
   {name: 'libgdk-pixbuf2.0-0', action: 'remove'},
 ]
else
  [
   {name: 'python', action: 'upgrade'},
   {name: 'unzip', action: 'upgrade'},
   {name: 'ghostscript', action: 'upgrade'},
   {name: 'wkhtmltopdf', options: '-f', action: 'upgrade'},
   {name: 'imagemagick-6.q16', action: 'upgrade'},
   {name: 'nodejs', action: 'upgrade'},
   {name: 'subversion', action: 'remove'},
   {name: 'libX11', action: 'remove'},
   {name: 'libtirpc', action: 'remove'},
   {name: 'apache', action: 'remove'},
   {name: 'libXi', action: 'remove'},
   {name: 'libXfixes', action: 'remove'},
   {name: 'libXpm', action: 'remove'},
   {name: 'libXrender', action: 'remove'},
   {name: 'libICE', action: 'remove'},
   {name: 'libxdmcp6', action: 'remove'},
   {name: 't1lib', action: 'remove'},
   {name: 'libgdk-pixbuf2.0-0', action: 'remove'},
  ]
end




default['ec_security_updates']['bleeding_edge_packages'] = case node['platform']
  when 'ubuntu' then
    [
      {name: 'cups-filters', version: '1.22.5-1', action: 'upgrade'},
      {name: 'poppler-utils', version: '0.74.0-0ubuntu1', action: 'upgrade'},
      {name: 'libjpeg-turbo8', version: '2.0.1-0ubuntu2', action: 'upgrade'},
      {name: 'jbig2dec', version: '0.15-2', action: 'upgrade'},
    ]
  when 'centos' then
    [
      {name: 'cups-filters', action: 'upgrade'},
      {name: 'poppler-utils', action: 'upgrade'},
      {name: 'libjpeg-turbo8', action: 'upgrade'},
      {name: 'jbig2dec', action: 'upgrade'},
    ]
else
  [
    {name: 'cups-filters', action: 'upgrade'},
    {name: 'poppler-utils', action: 'upgrade'},
    {name: 'libjpeg-turbo8', action: 'upgrade'},
    {name: 'jbig2dec', action: 'upgrade'},
  ]
end
