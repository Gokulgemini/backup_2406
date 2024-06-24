# v2(
#   fonts:
#     [
#       {
#         name: "MrsSaintDelafield",
#         default: false,
#         filename: "MrsSaintDelafield-Regular.ttf",
#         url: "https://s3.amazonaws.com/verifyvalid_resources/MrsSaintDelafield-Regular.ttf"
#       },
#      {
#         name: "Micr Font otf",
#         default: true,
#         filename: "MICRE13B-BM1.otf",
#         url: "https://s3.amazonaws.com/verifyvalid_resources/MICRE13B-BM1.otf"
#       },
#       {
#         name: "Micr Font ttf",
#         default: false,
#         filename: "MICRE13B-BM1.ttf",
#         url: "https://s3.amazonaws.com/verifyvalid_resources/MICRE13B-BM1.ttf"
#       }
#     ]
# )

default['fonts']['mrs_saint_delafield_s3_url'] = 'https://s3.amazonaws.com/verifyvalid_resources/MrsSaintDelafield-Regular.ttf'
default['fonts']['mrs_saint_delafield_file_name'] = 'MrsSaintDelafield-Regular.ttf'
default['fonts']['micr_otf_s3_url'] = 'https://s3.amazonaws.com/verifyvalid_resources/MICRE13B-BM1.otf'
default['fonts']['micr_otf_file_name'] = 'MICRE13B-BM1.otf'
default['fonts']['micr_ttf_s3_url'] = 'https://s3.amazonaws.com/verifyvalid_resources/MICRE13B-BM1.ttf'
default['fonts']['micr_ttf_file_name'] = 'MICRE13B-BM1.ttf'
